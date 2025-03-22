using PlayerInput.Model.Managers.InfoManager;
using PlayerInput.Model.Managers.MarketManager;
using PlayerInput.Model.Managers.PlayerManager;
using PlayerInput.Model.Managers.RemoteManager;
using PlayerInput.Model.Utils;
using Serilog;
using System.Xml.Linq;
using static PlayerInput.Model.Managers.PlayerManager.Status;

namespace PlayerInput.Model.Phases.ResourceBuyingPhase
{
    public class BuyStep
    {
        private readonly TaskCompletionSource<bool> _completedTcs = new();
        private readonly Player _active;
        private PurchaseData _purchaseData;

        public event Action<PurchaseData>? PurchaseUpdated;

        private readonly Dictionary<Button, Action> _buttonActions;

        public BuyStep(Player player)
        {
            _purchaseData = new();
            _active = player;
            PlayerManager.Instance.SetPlayerState(_active, PlayerState.Active);
            // Initialize button-action mappings
            _buttonActions = new()
            {
                { Button.BtnA, ActionA },
                { Button.BtnB, ActionB },
                { Button.BtnC, MoveUp },
                { Button.BtnD, MoveDown }
            };

            UpdateInfo();
        }

        public async Task Run() => await _completedTcs.Task;

        public void MoveUp() => 
            _purchaseData.Selected = (_purchaseData.Selected > 0) ? 
                _purchaseData.Selected - 1 : _purchaseData.PurchaseRecords.Count;

        public void MoveDown() => 
            _purchaseData.Selected = (_purchaseData.Selected < _purchaseData.PurchaseRecords.Count) ? 
                _purchaseData.Selected + 1 : 0;

        public void ActionA()
        {
            Log.Information("BtnA was pressed");
            if (IsLastRow())
            {
                ResetPhase();
                return;
            }

            Buy((ResourceType)_purchaseData.Selected);
        }

        public void ActionB()
        {
            if (IsLastRow())
            {
                CompletePhase();
                return;
            }

            Sell((ResourceType)_purchaseData.Selected);
        }

        public void ResetPhase()
        {
            App.LogPanelViewModel.Add($"{_active.Name}: bought {string.Join(", ", _purchaseData.PurchaseRecords)} with {_purchaseData.Total} electra");
            _purchaseData = new();
            PurchaseUpdated?.Invoke(_purchaseData);
        }

        public void CompletePhase()
        {
            ResetPhase();
            PlayerManager.Instance.SetPlayerState(_active, PlayerState.Done);
            _completedTcs.SetResult(true);
        }

        private bool IsLastRow() => _purchaseData.Selected == _purchaseData.PurchaseRecords.Count;

        public void HandleButtonPressed(Player player, Button btn)
        {
            Log.Information("HandleButtonPressed {playerName}, {btn}, {state}", player.Name, btn, player.Status.State);
            if (player != _active || player.Status.State != PlayerState.Active) 
                return;

            if (_buttonActions.TryGetValue(btn, out var action))
            {
                action.Invoke();
                PurchaseUpdated?.Invoke(_purchaseData);
            }
        }

        public void UpdateInfo()
        {
            InfoManager.Instance.PhaseName = "Buy Resources";
            InfoManager.Instance.PlayerName = _active.Name;
            InfoManager.Instance.OptionA = "+1/Ready";
            InfoManager.Instance.OptionB = "-1/Done";
            InfoManager.Instance.OptionC = "Up";
            InfoManager.Instance.OptionD = "Down";
        }
        
        public void UpdatePurchaseRecord(ResourceType type, int amount)
        {
            Log.Information("{ResourceType} {Amount}", type, amount);

            (amount < 0 ? (Action<ResourceType>)Sell : Buy).Invoke(type);

            PurchaseUpdated?.Invoke(_purchaseData);
        }

        private void Sell(ResourceType type)
        {
            if (_purchaseData.PurchaseRecords[(int)type] == 0)
                return;

            _purchaseData.Total -= MarketManager.Instance.Sell(type);
            _purchaseData.PurchaseRecords[(int)type]--;
        }

        private void Buy(ResourceType type) 
        {
            if (!MarketManager.Instance.HasStock(type))
                return;

            _purchaseData.Total += MarketManager.Instance.Buy(type);
            _purchaseData.PurchaseRecords[(int)type]++;
        }
    }
}

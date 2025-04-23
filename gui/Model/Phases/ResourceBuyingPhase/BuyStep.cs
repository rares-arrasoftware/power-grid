using gui.Model.Managers.InfoManager;
using gui.Model.Managers.MarketManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Managers.RemoteManager;
using gui.Model.Utils;
using Serilog;
using System.Xml.Linq;
using static gui.Model.Managers.PlayerManager.Status;

namespace gui.Model.Phases.ResourceBuyingPhase
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
            _active = player;

            _purchaseData = new PurchaseData
            {
                PurchaseRecords = player.SupportedResources()
                    .ToDictionary(resource => resource, _ => 0)
            };

            PlayerManager.Instance.SetPlayerState(_active, PlayerState.Active);
            // Initialize button-action mappings
            _buttonActions = new()
            {
                { Button.BtnA, ActionA },
                { Button.BtnB, ActionB },
                { Button.BtnC, Next },
                { Button.BtnD, CompletePhase }
            };
        }

        public void Init()
        {
            UpdateInfo();
            PurchaseUpdated?.Invoke(_purchaseData);
            if (_purchaseData.PurchaseRecords.Count == 0)
            {
                CompletePhase();
            }
        }

        public async Task Run() => await _completedTcs.Task;

        public void Next() => 
            _purchaseData.Selected = (_purchaseData.Selected < _purchaseData.PurchaseRecords.Count - 1) ? 
                _purchaseData.Selected + 1 : 0;

        public void ActionA()
        {
            Buy((ResourceType)_purchaseData.Selected);
        }

        public void ActionB()
        {
            Sell((ResourceType)_purchaseData.Selected);
        }

        public void CompletePhase()
        {
            PlayerManager.Instance.SetPlayerState(_active, PlayerState.Done);
            _completedTcs.SetResult(true);
        }

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
            InfoManager.Instance.OptionA = "+1";
            InfoManager.Instance.OptionB = "-1";
            InfoManager.Instance.OptionC = "Next";
            InfoManager.Instance.OptionD = "Done";
        }
        
        public void UpdatePurchaseRecord(ResourceType type, int amount)
        {
            Log.Information("{ResourceType} {Amount}", type, amount);

            (amount < 0 ? (Action<ResourceType>)Sell : Buy).Invoke(type);

            PurchaseUpdated?.Invoke(_purchaseData);
        }

        private void Sell(ResourceType type)
        {
            if (_purchaseData.PurchaseRecords[type] == 0)
                return;

            _purchaseData.Total -= MarketManager.Instance.Sell(type);
            _purchaseData.PurchaseRecords[type]--;
        }

        private void Buy(ResourceType type) 
        {
            if (!MarketManager.Instance.HasStock(type))
                return;

            _purchaseData.Total += MarketManager.Instance.Buy(type);
            _purchaseData.PurchaseRecords[type]++;
        }
    }
}

using PlayerInput.Model.Managers.MarketManager;
using PlayerInput.Model.Managers.PlayerManager;
using PlayerInput.Model.Managers.RemoteManager;
using PlayerInput.Model.Phases.AuctionPhase;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerInput.Model.Phases.ResourceBuyingPhase
{
    public class ResourceBuyingPhase(Action<PurchaseData>? purchaseUpdated) : Phase
    { 
        private Action<PurchaseData>? _purchaseUpdated = purchaseUpdated;

        private BuyStep? _buyStep;

        public override async Task Execute()
        {
            Log.Information("Running: ResourceBuyingPhase");
            var players = new Stack<Player>(PlayerManager.Instance.GetPlayers());

            while (players.Count > 0)
            {
                var player = players.Pop();
                _buyStep = new BuyStep(player);
                RemoteManager.Instance.ButtonPressed += _buyStep.HandleButtonPressed;
                _buyStep.PurchaseUpdated += OnPurchaseUpdated;
                await _buyStep.Run();
                RemoteManager.Instance.ButtonPressed -= _buyStep.HandleButtonPressed;
            }
        }

        public void OnPurchaseUpdated(PurchaseData purchaseData)
        {
            _purchaseUpdated?.Invoke(purchaseData);
        }

        public void UpdatePurchaseRecord(ResourceType type, int amount)
        {
            _buyStep?.UpdatePurchaseRecord(type, amount);
        }

        public void DonePurchase()
        {
            Log.Information("DonePurchase"); 
            _buyStep?.CompletePhase();
        }

        public void ReadyPurchase()
        {
            Log.Information("ReadyPurchase");
            _buyStep?.ResetPhase();
        }
    }
}


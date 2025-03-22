using PlayerInput.Model.Managers.MarketManager;
using PlayerInput.Model.Managers.PlayerManager;
using PlayerInput.Model.Phases;
using PlayerInput.Model.Phases.AuctionPhase;
using PlayerInput.Model.Phases.BureaucracyPhase;
using PlayerInput.Model.Phases.CityBuildingPhase;
using PlayerInput.Model.Phases.ResourceBuyingPhase;
using Serilog;

namespace PlayerInput.Model
{
    public class Round()
    {
        public event Action<Phase>? PhaseChanged;

        public event Action<PurchaseData>? PurchasedUpdated;
        
        public event Action<int, Player>? BuildUpdated;

        public Phase? CurrentPhase { get; set; }

        public async Task Start()
        {
            List<Phase> phases =
            [
                new AuctionPhase(),
                new ResourceBuyingPhase(PurchasedUpdated),
                new CityBuildingPhase(BuildUpdated),
                new BureaucracyPhase()
            ];

            foreach (Phase phase in phases)
            {
                PlayerManager.Instance.SetStateAll(Status.PlayerState.Wait);
                CurrentPhase = phase;
                PhaseChanged?.Invoke(phase);
                Log.Information($"Starting {phase.GetType().Name}");
                App.LogPanelViewModel.Add($"Starting {phase.GetType().Name}");
                await phase.Execute(); // Wait for the phase to finish
                Log.Information($"{phase.GetType().Name} completed");
            }
        }

        public void UpdatePurchaseRecord(ResourceType type, int amount)
        {
            if (CurrentPhase is ResourceBuyingPhase phase)
            {
                Log.Information("UpdatePurchaseRecord when phase is ResourceBuyingPhase");
                phase.UpdatePurchaseRecord(type, amount);
            }
        }

        public void UpdateBuild(int amount)
        {
            if (CurrentPhase is CityBuildingPhase phase)
            {
                Log.Information("UpdatePurchaseRecord when phase is ResourceBuyingPhase");
                phase.UpdateBuild(amount);
            }
        }

        public void Done()
        {
            if (CurrentPhase is ResourceBuyingPhase buyPhase)
            {
                Log.Information($"Done purchase {CurrentPhase}");
                buyPhase.DonePurchase();
            }
            if(CurrentPhase is CityBuildingPhase buildPhase)
            {
                Log.Information($"Done build {CurrentPhase}");
                buildPhase.DoneBuild();
            }
        }

        public void Ready()
        {
            if (CurrentPhase is ResourceBuyingPhase phase)
            {
                Log.Information($"Ready purchase {CurrentPhase}");
                phase.ReadyPurchase();
            }
            if (CurrentPhase is CityBuildingPhase buildPhase)
            {
                Log.Information($" Ready build {CurrentPhase}");
                buildPhase.ReadyBuild();
            }
        }
    }
}

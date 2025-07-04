using gui.Model.Managers.MarketManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Phases;
using gui.Model.Phases.AuctionPhase;
using gui.Model.Phases.BureaucracyPhase;
using gui.Model.Phases.CityBuildingPhase;
using gui.Model.Phases.ResourceBuyingPhase;
using Serilog;

namespace gui.Model
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
            if(CurrentPhase is BureaucracyPhase bureaucracyPhase)
            {
                Log.Information($"Done bureacracy {CurrentPhase}");
                bureaucracyPhase.OnStartRoundPressed();
            }
        }

        public void Ready()
        {
            if (CurrentPhase is CityBuildingPhase buildPhase)
            {
                Log.Information($" Ready build {CurrentPhase}");
                buildPhase.ReadyBuild();
            }
        }
    }
}

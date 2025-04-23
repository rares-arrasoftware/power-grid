using gui.Model.Managers.PlayerManager;
using gui.Model.Managers.RemoteManager;
using Serilog;

namespace gui.Model.Phases.AuctionPhase
{
    public class AuctionPhase() : Phase
    {
        private AuctionContext _ctx = new();

        public override Task Execute()
        {
            var tcs = new TaskCompletionSource<bool>();

            Log.Information("Running: AuctionPhase");

            RemoteManager.Instance.ButtonPressed += OnButtonPressed;

            // Run the task asynchronously on a background thread
            Task.Run(async () =>
            {
                await RunSteps();
                RemoteManager.Instance.ButtonPressed -= OnButtonPressed;
                tcs.SetResult(true); // Signal that the phase is done
            });

            return tcs.Task;
        }

        private async Task RunSteps()
        {
            Step? step = new StartAuctionStep(_ctx);
            while (step != null)
            {
                Log.Information("Running: {Step}", step.ToString());

                var currentStep = step;
                step = await currentStep.Execute();

                if (currentStep is IDisposable disposableStep)
                {
                    disposableStep.Dispose();
                    Log.Information("Disposed step: {Step}", currentStep);
                }
            }
        }

        public void OnButtonPressed(Player player, Button btn)
        {
            Log.Information("AuctionPhase:OnButtonPressed");
            if(btn == Button.BtnC) 
                _ctx.SpecialAuctionRequest.Enqueue(player);
        }
    }
}

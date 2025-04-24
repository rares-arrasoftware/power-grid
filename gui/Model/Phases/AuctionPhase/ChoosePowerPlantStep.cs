using gui.Model.Managers.CardManager;
using gui.Model.Managers.InfoManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Managers.RemoteManager;
using Serilog;
using System.IO;
using System.Media;
using System.Windows;
using static gui.Model.Managers.PlayerManager.Status;

namespace gui.Model.Phases.AuctionPhase
{
    public class ChoosePowerPlantStep(AuctionContext ctx) : Step(ctx)
    {
        public override async Task<Step?> Execute()
        {
            if (_ctx.Participants.Count == 0) 
                return null;

            var path = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds", "attention.wav");
            using (var media = new SoundPlayer(path))
            {
                media.Play(); // or PlaySync() if you want to block
            }


            Log.Information("Participants count: {ParticipantsCount}", _ctx.Participants.Count);

            _ctx.Participants.ForEach(p => PlayerManager.Instance.SetPlayerState(p, PlayerState.Wait));
            PlayerManager.Instance.SetPlayerState(_ctx.Participants.First(), PlayerState.Active);

            UpdateInfo();

            Log.Information("{Player} to act.", _ctx.Participants.First().Name);

            return await _stepCompletion.Task;
        }

        public override void HandleButtonPressed(Player player, Button btn)
        {
            Log.Information("HandleButtonPressed {PlayerName}, {Button}, {PlayerState}", player.Name, btn, player.Status.State.ToString());


            if (btn == Button.BtnC)
            {
                _stepCompletion.TrySetResult(new StartAuctionStep(_ctx));
                return;
            }
               
            if (player is not { Status.State: PlayerState.Active }) 
                return;

            if (btn != Button.BtnB)
                return;

            PlayerManager.Instance.SetPlayerState(player, PlayerState.Done);
            _ctx.DonePlayers.Add(player);

            _stepCompletion.TrySetResult(new StartAuctionStep(_ctx));
        }

        public override void HandleNewCardScanned(Card card)
        {
            _ctx.Card = card;

            if (card.Type == CardType.Event || card.Type == CardType.Utility) 
                return;

            if (card.EndsTurn)
            {
                PlayerManager.Instance.SetStateAll(PlayerState.Wait);
                _ctx.Participants = PlayerManager.Instance.GetPlayers();
                _ctx.SpecialAuctionRequest.Clear();
            }
                
            _stepCompletion.TrySetResult(new BidOrPassStep(_ctx));
        }

        public void UpdateInfo()
        {
            Log.Information("UpdateInfo running on thread: {ThreadId}", Environment.CurrentManagedThreadId);

            InfoManager.Instance.PhaseName = "Auction: Choose Plant";
            InfoManager.Instance.PlayerName = _ctx.Participants[0].Name;
            InfoManager.Instance.OptionA = "N/A";
            InfoManager.Instance.OptionB = "Pass";
            InfoManager.Instance.OptionC = "SpecialAuction";
            InfoManager.Instance.OptionD = "N/A";
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using gui.Model.Managers.CardManager;
using gui.Model.Managers.InfoManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Managers.RemoteManager;
using Serilog;
using static gui.Model.Managers.PlayerManager.Status;

namespace gui.Model.Phases.AuctionPhase
{
    public class ChooseUtilityCardStep(AuctionContext ctx) : Step(ctx)
    {
        public override async Task<Step?> Execute()
        {
            _ctx.Participants.ForEach(p => PlayerManager.Instance.SetPlayerState(p, PlayerState.Wait));
            Player player = _ctx.SpecialAuctionRequest.Dequeue();
            PlayerManager.Instance.SetPlayerState(player, PlayerState.Active);

            var path = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds", "attention.wav");
            using (var media = new SoundPlayer(path))
            {
                media.Play(); // or PlaySync() if you want to block
            }


            UpdateInfo(player);

            Log.Information("{Player} to act.", player.Name);

            return await _stepCompletion.Task;
        }

        public override void HandleButtonPressed(Player player, Button btn)
        {
            if (btn != Button.BtnB)
                return;

            if (player is not { Status.State: PlayerState.Active })
                return;

            _stepCompletion.TrySetResult(new StartAuctionStep(_ctx));
        }

        public override void HandleNewCardScanned(Card card)
        {
            if (card.Type != CardType.Utility)
                return;

            _ctx.Card = card;
            _stepCompletion.TrySetResult(new BidOrPassStep(_ctx));
        }

        public void UpdateInfo(Player player)
        {
            InfoManager.Instance.PhaseName = "Auction: Choose Special";
            InfoManager.Instance.PlayerName = player.Name;
            InfoManager.Instance.OptionA = "N/A";
            InfoManager.Instance.OptionB = "Pass";
            InfoManager.Instance.OptionC = "SpecialAuction";
            InfoManager.Instance.OptionD = "N/A";
        }
    }
}

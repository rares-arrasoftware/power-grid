using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerInput.Model.Managers.CardManager;
using PlayerInput.Model.Managers.InfoManager;
using PlayerInput.Model.Managers.PlayerManager;
using PlayerInput.Model.Managers.RemoteManager;
using Serilog;
using static PlayerInput.Model.Managers.PlayerManager.Status;

namespace PlayerInput.Model.Phases.AuctionPhase
{
    public class SubmitBidStep(AuctionContext ctx) : Step(ctx)
    {
        public override async Task<Step?> Execute()
        {
            _ctx.Participants.ForEach(p => PlayerManager.Instance.SetPlayerState(p, PlayerState.Active));
            Log.Information("{PlayersCount} players to act.", _ctx.Participants.Count);

            UpdateInfo();

            return await _stepCompletion.Task;
        }

        public override void HandleButtonPressed(Player player, Button btn)
        {
            if (player is not { Status.State: PlayerState.Active })
                return;

            if (btn == Button.BtnA)
                player.Status.State = PlayerState.Ready;

            UpdateInfo();

            if (PlayerManager.Instance.CountByState(PlayerState.Active) > 0)
                return;

            _stepCompletion.TrySetResult(new ResolveWinnerStep(_ctx));
        }

        public void UpdateInfo()
        {
            InfoManager.Instance.PhaseName = "Auction: Submit Bid";
            InfoManager.Instance.PlayerName = $"{PlayerManager.Instance.CountByState(PlayerState.Active)} Players";
            InfoManager.Instance.OptionA = "Ready";
            InfoManager.Instance.OptionB = "N/A";
            InfoManager.Instance.OptionC = "SpecialAuction";
            InfoManager.Instance.OptionD = "N/A";
        }

        public override void HandleNewCardScanned(Card card)
        {
            _ctx.Card = card;
            _stepCompletion.TrySetResult(new StartAuctionStep(_ctx));
        }
    }
}

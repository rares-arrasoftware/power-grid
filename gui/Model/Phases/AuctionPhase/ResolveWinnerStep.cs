using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ResolveWinnerStep(AuctionContext ctx) : Step(ctx)
    {
        // Dictionary for button handling
        private readonly Dictionary<Button, Action<Player>> _buttonActions = new()
        {
            { Button.BtnA, p => PlayerManager.Instance.SetPlayerState(p, PlayerState.Ready) },
            { Button.BtnB, p => PlayerManager.Instance.SetPlayerState(p, PlayerState.Wait) }
        };

        public override async Task<Step?> Execute()
        {
            _ctx.Participants.ForEach(p => p.Status.State = PlayerState.Active);
            Log.Information("{PlayersCount} players to act.", _ctx.Participants.Count);

            if (_ctx.Participants.Count <= 1)
            {
                return new ThrowCardStep(_ctx);
            }

            UpdateInfo();

            return await _stepCompletion.Task;
        }

        public override void HandleButtonPressed(Player player, Button btn)
        {
            if (player is not { Status.State: PlayerState.Active })
                return;

            // Handle button action
            if (_buttonActions.TryGetValue(btn, out var action))
                action(player);

            UpdateInfo();

            if (PlayerManager.Instance.CountByState(PlayerState.Active) > 0)
                return;
                
            // prepare for next step
            int readyCount = PlayerManager.Instance.CountByState(PlayerState.Ready);
            _ctx.Participants = PlayerManager.Instance.GetPlayersByState(PlayerState.Ready);

            _stepCompletion.TrySetResult(
                readyCount > 1 ? new SubmitBidStep(_ctx) : new ThrowCardStep(_ctx));

        }

        public void UpdateInfo()
        {
            InfoManager.Instance.PhaseName = "Auction: Winner";
            InfoManager.Instance.PlayerName = $"{PlayerManager.Instance.CountByState(PlayerState.Active)} Players";
            InfoManager.Instance.OptionA = "I've Won/Tie";
            InfoManager.Instance.OptionB = "I've Lost";
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

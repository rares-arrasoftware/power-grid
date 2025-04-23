using gui.Model.Managers.CardManager;
using gui.Model.Managers.InfoManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Managers.RemoteManager;
using Serilog;
using System.Numerics;
using static gui.Model.Managers.PlayerManager.Status;

namespace gui.Model.Phases.AuctionPhase
{
    public class ResolveWinnerStep(AuctionContext ctx) : Step(ctx)
    {
        public override async Task<Step?> Execute()
        {
            _ctx.Participants.ForEach(p => p.Status.State = PlayerState.Ready);
            Log.Information("{PlayersCount} players to act.", _ctx.Participants.Count);

            if (_ctx.Participants.Count == 0)
            {
                return new ThrowCardStep(_ctx);
            }

            if (_ctx.Participants.Count == 1)
            {
                PlayerManager.Instance.SetPlayerState(_ctx.Participants.First(), PlayerState.Active);
                return new ThrowCardStep(_ctx);
            }

            UpdateInfo();

            return await _stepCompletion.Task;
        }

        public override void HandleButtonPressed(Player player, Button btn)
        {
            if (player is not { Status.State: PlayerState.Ready })
                return;

            if (btn != Button.BtnD)
                return;

            PlayerManager.Instance.SetPlayerState(player, PlayerState.Active);
            _ctx.Participants = PlayerManager.Instance.GetPlayersByState(PlayerState.Active);

            if(_ctx.Card != null && _ctx.Card.EndsTurn)
            {
                _ctx.DonePlayers.AddRange(
                    _ctx.Participants.Where(p => p.Status.State == PlayerState.Ready));
            }

            _stepCompletion.TrySetResult(new ThrowCardStep(_ctx));
        }

        public void UpdateInfo()
        {
            InfoManager.Instance.PhaseName = "Auction: Winner";
            InfoManager.Instance.PlayerName = $"{PlayerManager.Instance.CountByState(PlayerState.Ready)} Players";
            InfoManager.Instance.OptionA = "N/A";
            InfoManager.Instance.OptionB = "N/A";
            InfoManager.Instance.OptionC = "SpecialAuction";
            InfoManager.Instance.OptionD = "Winner";
        }

        public override void HandleNewCardScanned(Card card)
        {
            _ctx.Card = card;
            _stepCompletion.TrySetResult(new StartAuctionStep(_ctx));
        }
    }
}

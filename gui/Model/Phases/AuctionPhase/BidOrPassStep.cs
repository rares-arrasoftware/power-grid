﻿using gui.Model.Managers.CardManager;
using gui.Model.Managers.InfoManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Managers.RemoteManager;
using Serilog;
using static gui.Model.Managers.PlayerManager.Status;

namespace gui.Model.Phases.AuctionPhase
{
    public class BidOrPassStep(AuctionContext ctx) : Step(ctx)
    {
        // Dictionary for button handling
        private readonly Dictionary<Button, Action<Player>> _buttonActions = new()
        {
            { Button.BtnA, p => PlayerManager.Instance.SetPlayerState(p, PlayerState.Ready) },
            { Button.BtnB, p => PlayerManager.Instance.SetPlayerState(p, PlayerState.Wait) }
        };

        public override async Task<Step?> Execute()
        {
            foreach (var p in _ctx.Participants)
            {
                var newState = (p.Status.State == PlayerState.Active) ? PlayerState.Ready : PlayerState.Active;
                PlayerManager.Instance.SetPlayerState(p, newState);
            }

            if (_ctx.Participants.Count == 1)
                return new ResolveWinnerStep(_ctx);

            UpdateInfo();

            Log.Information("{PlayersCount} players to act.", _ctx.Participants.Count);

            return await _stepCompletion.Task;
        }

        public override void HandleButtonPressed(Player player, Button btn)
        {
            if (_ctx.Card == null)
            {
                _stepCompletion.TrySetResult(new StartAuctionStep(_ctx));
                return;
            }

            // Player not active, disregard his action
            if (player is not { Status.State: PlayerState.Active }) 
                return;

            // Handle button action
            if (_buttonActions.TryGetValue(btn, out var action))
                action(player);

            UpdateInfo();

            // check if we still have active players 
            if (PlayerManager.Instance.CountByState(PlayerState.Active) > 0) 
                return;

            // prepare for next step
            
            if(_ctx.Card.EndsTurn)
            {
                _ctx.DonePlayers.AddRange(
                    _ctx.Participants.Where(p => p.Status.State == PlayerState.Wait));
            }

            _ctx.Participants = PlayerManager.Instance.GetPlayersByState(PlayerState.Ready);
            _stepCompletion.TrySetResult(new ResolveWinnerStep(_ctx));
        }

        public void UpdateInfo()
        {
            InfoManager.Instance.PhaseName = "Auction: Bid or Pass?";
            InfoManager.Instance.PlayerName = $"{PlayerManager.Instance.CountByState(PlayerState.Active)} Players";
            InfoManager.Instance.OptionA = "Bid";
            InfoManager.Instance.OptionB = "Pass";
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

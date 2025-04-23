using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public override async Task<Step?> Execute()
        {
            _ctx.Participants.ForEach(p => p.Status.State = PlayerState.Ready);
            Log.Information("{PlayersCount} players to act.", _ctx.Participants.Count);

            if (_ctx.Participants.Count <= 1)
            {
                AssignWinner(_ctx.Participants.First());
                return new StartAuctionStep(_ctx);
            }

            UpdateInfo();

            return await _stepCompletion.Task;
        }

        public override void HandleButtonPressed(Player player, Button btn)
        {
            if (player is not { Status.State: PlayerState.Ready })
                return;

            if (btn == Button.BtnD)
                AssignWinner(player);

            _stepCompletion.TrySetResult(new StartAuctionStep(_ctx));
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

        void AssignWinner(Player player)
        {
            if (_ctx.Card == null) 
                return;
            
            // Assign Card
            CardManager.Instance.AssignCard(player, _ctx.Card);

            // Utility Card
            if(_ctx.Card.Type == CardType.Utility)
            {
                if (_ctx.Card.Bureaucrat)
                    PlayerManager.Instance.ApplyBureaucrat();
                return;
            }
            
            // Power Plant Card
            PlayerManager.Instance.SetPlayerState(player, PlayerState.Done);
            _ctx.DonePlayers.Add(player);
        }

        public override void HandleNewCardScanned(Card card)
        {
            _ctx.Card = card;
            _stepCompletion.TrySetResult(new StartAuctionStep(_ctx));
        }
    }
}

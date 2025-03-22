using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerInput.Model.Managers.PlayerManager;
using Serilog;
using static PlayerInput.Model.Managers.PlayerManager.Status;

namespace PlayerInput.Model.Phases.AuctionPhase
{
    public class StartAuctionStep(AuctionContext ctx) : Step(ctx)
    {
        public override Task<Step?> Execute()
        {
            Log.Information("Starting auction");
            bool isSpecialAuction = _ctx.SpecialAuctionRequest.Count > 0;

            if (isSpecialAuction)
            {
                // everybody is invited
                _ctx.Participants = PlayerManager.Instance.GetPlayers();
                return Task.FromResult<Step?>(new ChooseUtilityCardStep(_ctx));
            }

            _ctx.Participants = PlayerManager.Instance.GetPlayersExcept(_ctx.DonePlayers);
            _ctx.DonePlayers.ForEach(p => PlayerManager.Instance.SetPlayerState(p, PlayerState.Done));
            return Task.FromResult<Step?>(new ChoosePowerPlantStep(_ctx));     
        }
    }
}

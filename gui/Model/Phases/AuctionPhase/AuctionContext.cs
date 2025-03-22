using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerInput.Model.Managers.CardManager;
using PlayerInput.Model.Managers.PlayerManager;

namespace PlayerInput.Model.Phases.AuctionPhase
{
    public class AuctionContext()
    {
        public Card? Card = null;

        public List<Player> DonePlayers = [];

        public Queue<Player> SpecialAuctionRequest = [];

        public List<Player> Participants = [];
    }
}

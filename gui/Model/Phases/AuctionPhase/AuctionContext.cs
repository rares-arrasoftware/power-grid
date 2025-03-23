using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gui.Model.Managers.CardManager;
using gui.Model.Managers.PlayerManager;

namespace gui.Model.Phases.AuctionPhase
{
    public class AuctionContext()
    {
        public Card? Card = null;

        public List<Player> DonePlayers = [];

        public Queue<Player> SpecialAuctionRequest = [];

        public List<Player> Participants = [];
    }
}

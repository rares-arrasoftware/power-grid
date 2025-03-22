using PlayerInput.Model.Managers.MarketManager;
using PlayerInput.Model.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace PlayerInput.Model.Managers.CardManager
{
    public enum CardType
    {
        PowerPlant,
        Event,
        Utility
    }

    public class Card
    {
        public int Id { get; set; }
        public CardType Type { get; set; }
        public int Rank { get; set; } = 0;
        public string ImagePath { get; set; } = "";

        // Properties for different card types
        public bool Plus { get; set; } = false;         // PowerPlant only
        public bool EndsTurn { get; set; } = false;     // PowerPlant only
        public bool Bureaucrat { get; set; } = false;   // Utility only
        public bool Level3 { get; set; } = false;       // Event only

        // Market effect list (matches the resource order)
        public List<int> MarketEffect { get; set; } = ListUtils.EnumToList<ResourceType, int>(_ => 0);

        public int MarketEffectLowest { get; set; } = 0;

        public override string ToString()
        {
            return $"{Type} Card [ID: {Id}, Rank: {Rank}, Image: {ImagePath}]";
        }
    }
}

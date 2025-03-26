using gui.Model.Managers.MarketManager;
using gui.Model.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace gui.Model.Managers.CardManager
{
    public enum CardType
    {
        PowerPlant,
        Event,
        Utility
    }

    public class Card
    {
        // Core properties
        public int Id { get; set; }
        public CardType Type { get; set; }
        public int Rank { get; set; } = 0;
        public string ImagePath { get; set; } = "";

        // PowerPlant specific
        public bool Plus { get; set; } = false;
        public bool EndsTurn { get; set; } = false;

        // Event specific
        public bool Level3 { get; set; } = false;

        // Utility specific
        public bool Bureaucrat { get; set; } = false;

        // Market effects
        public List<int> MarketEffect { get; set; } = ListUtils.EnumToList<ResourceType, int>(_ => 0);
        public int MarketEffectLowest { get; set; } = 0;

        public override string ToString() => $"{Type} Card [ID: {Id}, Rank: {Rank}, Image: {ImagePath}]";
    }
}

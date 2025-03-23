using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gui.Model.Managers.MarketManager
{
    public class Resource(ResourceType type, int price, int id)
    {
        // 1. Fields
        private bool _filled = false;
        public bool Filled
        {
            get => _filled;
            set
            {
                if (_filled != value)
                {
                    _filled = value;
                    StateChanged?.Invoke(this, EventArgs.Empty); // Notify listeners of state changes
                }
            }
        }

        public int Id { get; init; } = id;

        public int Price { get; init; } = price;

        public ResourceType Type { get; init; } = type;

        // 2. Events
        public event EventHandler? StateChanged;
    }
}

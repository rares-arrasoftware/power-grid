using PlayerInput.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerInput.Model.Managers.MarketManager
{
    public class PriceTier(int price)
    {
        // 1. Properties
        public int Price { get; private set; } = price;

        public List<List<Resource>> ResourcesByType { get; } = 
            ListUtils.EnumToList<ResourceType, List<Resource>>(_ => []);

        // 2. Public Methods
        public void Add(Resource resource)
        {
            ResourcesByType[(int)resource.Type].Add(resource);
        }
    }
}

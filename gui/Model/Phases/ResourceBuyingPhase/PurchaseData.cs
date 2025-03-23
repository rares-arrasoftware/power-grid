using gui.Model.Managers.MarketManager;
using gui.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gui.Model.Phases.ResourceBuyingPhase
{
    public class PurchaseData()
    {
        public int Selected = 0;
        public int Total = 0;
        public List<int> PurchaseRecords =
            ListUtils.EnumToList<ResourceType, int>(_ => 0);
    }
}

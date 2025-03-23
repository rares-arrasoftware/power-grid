using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gui.Model.Phases
{
    public abstract class Phase
    {
        public abstract Task Execute();

    }
}

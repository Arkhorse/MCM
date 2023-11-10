using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraMonitor.ModSettings.API.GUI
{
    public class MCMTab
    {
        public UIGrid Grid { get; set; }
        public Il2CppSystem.Collections.Generic.List<GameObject>? Items { get; set; }
        public Il2CppSystem.Collections.Generic.List<MCMMod>? MCMMods { get; set; }
        public float ScrollBarHeight { get; set; }
        public bool OverrideConfirmation { get; set; }

        public MCMTab(UIGrid grid, Il2CppSystem.Collections.Generic.List<GameObject>? Items)
        {
            this.Grid   = grid;
            this.Items  = Items;
            MCMMods     ??= new();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraMonitor.ModSettings.API.Attributes
{
    public class DescriptionAttribute : Attribute
    {
        public string? Title { get; set; }
        public bool Localize { get; set; } = false;

        public DescriptionAttribute(string title)
        {
            Title = title;
        }

        public DescriptionAttribute(string title, bool localize)
        {
            Title       = title;
            Localize    = localize;
        }
    }
}

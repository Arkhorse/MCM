using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraMonitor.ModSettings.API.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SectionAttribute : Attribute
    {
        public string? Title { get; set; }
        public bool Localize { get; set; } = false;
        public bool Collapse { get; set; } = false;

        public SectionAttribute(string title)
        {
            Title       = title;
        }

        public SectionAttribute(string title, bool localize)
        {
            Title       = title;
            Localize    = localize;
        }

        public SectionAttribute(string title, bool localize, bool collapse)
        {
            Title       = title;
            Localize    = localize;
            Collapse    = collapse;
        }
    }
}

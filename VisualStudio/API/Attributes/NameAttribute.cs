using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraMonitor.ModSettings.API.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class NameAttribute : Attribute
    {
        public string? Title { get; set; }
        public bool Localize { get; set; } = false;
        public NameAttribute(string title)
        {
            Title = title;
        }

        public NameAttribute(string title, bool localize)
        {
            Title       = title;
            Localize    = localize;
        }
    }
}

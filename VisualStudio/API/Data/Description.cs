using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraMonitor.ModSettings.API.Data
{
    [RegisterTypeInIl2Cpp(false)]
    public class Description : MonoBehaviour
    {
        public string? Text { get; private set; }

        public void Set(string text, bool localize)
        {
            if (localize)
            {
                Text = Localization.Get(text);
            }
            else
            {
                Text = text;
            }
        }
    }
}

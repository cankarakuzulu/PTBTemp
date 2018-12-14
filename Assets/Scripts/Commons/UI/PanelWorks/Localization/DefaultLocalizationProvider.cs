using UnityEngine;
using System.Collections;

namespace nopact.Commons.UI.PanelWorks.Localization
{
    public class DefaultLocalizationProvider :ILocalizationProvider
    {
        public string this[ string key ]
        {
            get
            {
                return "Missing localization";
            }
        }
    }
}

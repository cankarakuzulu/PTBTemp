using UnityEngine;
using System.Collections;

namespace nopact.Commons.UI.PanelWorks.Localization
{
    public interface ILocalizationProvider 
    {
        string this[string key ]
        {
            get;
        }
    }
}

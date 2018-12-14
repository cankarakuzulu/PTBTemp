using UnityEngine;
using System.Collections;
using nopact.Commons.UI.PanelWorks.Localization;

namespace nopact.Commons.UI.PanelWorks
{
    public abstract class UISetup 
    {

        public abstract void Setup( GameUI ui, UIParameters parameters, UICallbacks callbacks, ILocalizationProvider localization );
        public abstract void Kill();

    }
}

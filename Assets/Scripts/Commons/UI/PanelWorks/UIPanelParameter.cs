using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using nopact.Commons.UI.PanelWorks.Localization;

namespace nopact.Commons.UI.PanelWorks
{

    public class UIPanelParameter
    {

        public Dictionary<string, Action> ActionCallbacks { private get;  set; }
        
        private Localization.ILocalizationProvider localization;

        public Action GetAction ( string name )
        {

            Action action;

            if ( ActionCallbacks.TryGetValue( name, out action ) )
            {
                return action;
            }

            Debug.LogError( string.Format( "[Parameter] Action Callback key doesn't exist: {0}", name ) );
            return null;

        }
        public UIPanelParameter ( Localization.ILocalizationProvider localization )
        {
            this.localization = localization;
        }

        public ILocalizationProvider Localization
        {
            get
            {
                return localization;
            }
        }


    }

}

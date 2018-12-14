
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nopact.Commons.UI.PanelWorks
{
    public class UIPanelHolderComparer : IEqualityComparer<UIPanelHolder>
    {
        public bool Equals( UIPanelHolder x, UIPanelHolder y )
        {

            return x.Name.Equals( y.Name );

        }

        public int GetHashCode( UIPanelHolder item )
        {
            return item.Name.GetHashCode() ^ item.Panel.GetHashCode();
        }
    }

}

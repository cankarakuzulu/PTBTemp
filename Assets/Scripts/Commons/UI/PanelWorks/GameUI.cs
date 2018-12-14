using UnityEngine;
using System.Collections;
using nopact.Commons.Utility.Singleton;
using System;

#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
#endif

namespace nopact.Commons.UI.PanelWorks
{
    public class GameUI : MonoBehaviour
    {
        // Using wrapper class because unity cannot serialize dictionaries. But as there is a custom inspector for this class, it will look like a dictionary does.
        // Anyway, storing UI panels on a built-in array is faster. And this array will always be sorted key-wise on editor time by the custom inspector. 
        // At runtime, a sorted array optimized recursive find method ensures fastest access.

        [SerializeField] private UIPanelHolder[ ] panels;

#if UNITY_EDITOR
        public void SortPanels()
        {

            if ( panels == null || panels.Length < 1 )
            {

                return;

            }

            List<UIPanelHolder> panelList = panels.ToList();
            panelList.RemoveAll( item =>item == null || string.IsNullOrEmpty( item.Name ) );
            var distinctItems = panelList.Distinct( new UIPanelHolderComparer() ).ToList();
            panels = distinctItems.OrderBy( item => item.Name ).ToArray<UIPanelHolder>();        
            
        }
#endif
              
        // Warning, this method assumes that the panels array is sorted on editor time!
        /// <summary>
        ///  Finds the panel named <paramref name="name"/> of panels of type <typeparamref name="T"/> This is a pretty fast call. (No reflection, not much overhead)
        /// </summary>
        /// <typeparam name="T">Panel type. This avoids a type conversion.</typeparam>
        /// <param name="name">Panel name. This specification is made to selectively call different panels of the same type.</param>
        /// <returns></returns>
        public T GetPanel<T> ( string name ) where T: UIPanelBase
        {

            T panel = FindPanel(0, panels.Length, name) as T;

            if ( panel == default(T))
            {
                Debug.LogError( string.Format( "Cannot find UI panel:{0}", name ) );
                return null;
            }

            return panel;

        }

        private UIPanelBase FindPanel( int startIndex, int stopIndexExclusive, string name )
        {

            if ( startIndex == stopIndexExclusive )
            {
                // unable to find.
                return null;

            }

            int mid = ( stopIndexExclusive + startIndex ) / 2 ;
            UIPanelHolder middleElement = panels[ mid ];

            int compareResult =  name.CompareTo( middleElement.Name );

            if ( compareResult == 0 )
            {
                return middleElement.Panel;
            }
            else if ( compareResult < 0 )
            {
                return FindPanel( startIndex, mid, name );
            }
            else
            {
                return FindPanel( mid + 1, stopIndexExclusive, name );
            }

        }
    }

}

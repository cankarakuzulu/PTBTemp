using UnityEngine;
using System.Collections;

namespace nopact.Commons.UI.PanelWorks
{
    [System.Serializable]
    public class UIPanelHolder 
    {
        [SerializeField] private string name;
        [SerializeField] private UIPanelBase panel;

        public string Name { get { return name; } }
        public UIPanelBase Panel { get { return panel; } }

    }

}


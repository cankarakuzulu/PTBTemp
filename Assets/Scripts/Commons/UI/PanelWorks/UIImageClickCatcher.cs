using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace nopact.Commons.UI.PanelWorks
{
    public class UIImageClickCatcher :MonoBehaviour, IPointerClickHandler
    {

        [SerializeField] private UnityEvent onClick;

        public void OnPointerClick( PointerEventData eventData )
        {
            if ( onClick != null)
            {
                onClick.Invoke();
            }
        }
    }

}

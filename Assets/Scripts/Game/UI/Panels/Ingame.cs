using nopact.Commons.UI.PanelWorks;
using nopact.ShapeCutter.PlaySession;
using UnityEngine;

namespace nopact.Game.UI.Panels
{
    public class Ingame : StandardPanel
    {

        public void OnSuccessDummy()
        {
            config.GetAction("Success").Invoke();
        }

        public void OnFailDummy()
        {
            config.GetAction("Fail").Invoke();
        }
  
        public void OnPause()
        {
            config.GetAction("Pause").Invoke();
        }
    }
  
}


using nopact.Commons.UI.PanelWorks;

namespace nopact.Game.UI
{
    public class Main : StandardPanel
    {

        public void OnClickPlay()
        {
            config.GetAction("Play").Invoke();
        }
    }
}

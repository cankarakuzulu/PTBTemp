namespace nopact.Commons.UI.PanelWorks
{
    public abstract class StandardPanel : UIAnimatedPanel
    {
        protected UIPanelParameter config;
    
        public override void Setup<T>(T config)
        {
            this.config = config;
            uGameObject = gameObject;
            uGameObject.SetActive(false);
        }

        protected override void Activate()
        {
            uGameObject.SetActive(true);
            base.Activate();
        }

        protected override void Deactivate()
        {
            uGameObject.SetActive(false);
            base.Deactivate();
        }
    }
}
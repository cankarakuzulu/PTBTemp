using UnityEngine;
using System.Collections;
using nopact.Commons.Utility.Singleton;
using nopact.Commons.Utility.Debug;

namespace nopact.Commons.Utility
{
    public class DeviceManager :GenericSingleton<DeviceManager>
    {

        public float AspectRatio
        {
            get
            {
                return (float)Screen.width / Screen.height;
            }
        }

        protected override void Initialize()
        {

            base.Initialize();
            
        }
    }
}

using UnityEngine;

namespace nopact.Commons.Utility.Singleton
{
    public abstract class SingletonMonobehaviourBase : MonoBehaviour
    {
        public bool initialized;

        public abstract void Initialize();

        private void Awake()
        {
            
        }
    }
}

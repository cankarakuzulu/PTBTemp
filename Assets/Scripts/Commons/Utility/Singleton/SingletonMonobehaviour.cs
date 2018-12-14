using nopact.Commons.Utility.Threading;
using UnityEngine;

namespace nopact.Commons.Utility.Singleton
{
    public class SingletonMonobehaviour<T> where T : SingletonMonobehaviourBase
    {
        private static volatile T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (GenericLock<T>.SyncLock)
                    {
                        if (instance == null)
                        {
                            //find existing instance
                            instance = UnityEngine.Object.FindObjectOfType(typeof(T)) as T;

                            if (instance == null)
                            {
                                //create new instance
                                GameObject go = new GameObject(typeof(T).Name);
                                instance = go.AddComponent<T>();
                                Object.DontDestroyOnLoad(go);
                            }

                            //initialize instance if necessary
                            if (!instance.initialized)
                            {
                                instance.Initialize();
                                instance.initialized = true;
                            }
                        }
                    }
                }

                return instance;
            }
        }
    }
}

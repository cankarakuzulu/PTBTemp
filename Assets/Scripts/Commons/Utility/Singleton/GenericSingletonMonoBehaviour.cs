/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using nopact.Commons.Utility.Threading;
using UnityEngine;

namespace nopact.Commons.Utility.Singleton
{
    public class GenericSingletonMonoBehaviour<T> : MonoBehaviour where T : GenericSingletonMonoBehaviour<T>
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
                                DontDestroyOnLoad(go);
                            }

                            //initialize instance if necessary
                            if (instance != null && !instance.Initialized)
                            {
                                instance.Initialize();
                                instance.Initialized = true;
                            }
                        }
                    }
                }

                return instance;
            }
        }

        protected void Awake()
        {
            // Check if instance already exists when reloading original scene
            if (instance != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                InitializeOnAwake();
            }
        }

        protected bool Initialized { get; set; }

        protected virtual void Initialize() { }

        protected virtual void InitializeOnAwake() { }
    }
}

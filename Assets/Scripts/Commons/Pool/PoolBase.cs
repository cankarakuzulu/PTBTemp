using System;
using System.Collections.Generic;
using UnityEngine;

namespace nopact.Commons.Pool
{
    public abstract class PoolBase
    {

        public static event Action<IPoolable> OnPoolableReturn;
        
        protected HashSet<GameObject> outInuse;
        protected PoolSettings settings;
        protected BackoffEngine backoff;
        protected DateTime lastBackOffTime;
        protected bool isBackingOff;

        public abstract void Return( GameObject gameObject );
        public abstract GameObject Next<T>( out T poolable ) where T : MonoBehaviour, IPoolable;
        public abstract T GetPoolable<T>( GameObject clone ) where T : MonoBehaviour, IPoolable;        
        public abstract void RefreshBackoff();
        public abstract void Prepare();
        public abstract void ReturnAll();


        public static void TriggerPoolableReturn(IPoolable poolable)
        {
            if (OnPoolableReturn != null)
            {
                OnPoolableReturn(poolable);
            }
        }
        
        public virtual GameObject[ ] GetAllActive()
        {
            GameObject[ ] activeObjects = new GameObject[ outInuse.Count ];
            outInuse.CopyTo( activeObjects );
            return activeObjects;            
        }

        public PoolBase( PoolSettings settings )
        {
            this.settings = settings;
            outInuse = new HashSet<GameObject>();
        }

        public virtual uint Count { get { return (uint)outInuse.Count; } }
    }
}
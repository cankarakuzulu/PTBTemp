using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nopact.Commons.Pool.Exceptions;
using UnityEngine;

namespace nopact.Commons.Pool
{
    public class GenericPool<T> : PoolBase where T :  MonoBehaviour, IPoolable 
    {
        private Dictionary<GameObject, T> allLookup;
        private Queue<T> inUnused;
            
        public override void Prepare ()
        {
            for ( int objectIndex = 0; objectIndex < settings.Min; objectIndex ++ )
            {
                CreateNewObject();
            }
            if ( settings.BackoffProfile != PoolSettings.BackoffProfiles.None )
            {
                backoff = new BackoffEngine( settings.BackoffProfile );                
            }
        }
    
        public override void ReturnAll()
        {
            GameObject[ ] setElements = outInuse.ToArray();
            for( int setIndex = setElements.Length-1; setIndex >= 0; setIndex -- )
            {                
                GameObject toRemove = setElements[ setIndex ];
                Return( toRemove );
            }
            outInuse.Clear();            
        }

        public override void Return ( GameObject gameObject )
        {
            if ( !outInuse.Contains ( gameObject ) )
            {
                throw new PoolException( string.Format( "[Pooler] Pooler[{0}] cannot recycle {1}", settings.Prefab.name, gameObject.name ) );
            }
            T poolable;
            if ( allLookup.TryGetValue ( gameObject, out poolable ) )
            {
                ReturnObject( gameObject, poolable );
            }
            else
            {
                throw new PoolException( string.Format( "[Pooler] Pooler[{0}] cannot recycle {1}: Cannot find pairing behaviour.", settings.Prefab.name, gameObject.name ) );
            }
        }

        public override void RefreshBackoff()
        {
            if( !isBackingOff )
            {
                return;
            }
            var timePassed = DateTime.Now - lastBackOffTime;

            if ( timePassed.TotalMilliseconds < settings.BackoffRefreshRate * 1000 )
            {
                UpdateBackoff();
                lastBackOffTime = DateTime.Now;
            }
            
        }

        public override GameObject Next<K>( out K otherPoolable )
        {
            T poolable;
            var clone = NextGeneric( out poolable );
            otherPoolable = poolable as K;
            return clone; 
        }

        public GameObject NextGeneric( out T poolable )
        {
            if ( inUnused.Count > 0 )
            {
                poolable = inUnused.Dequeue();
                outInuse.Add( poolable.UGameObject );
                poolable.OnPoolEnable();

                return poolable.UGameObject;
            }
            Debug.Log( string.Format("[{0}] Pool over capacity. Creating new object.", settings.Prefab.name));
            CreateNewObject();
            return Next( out poolable );
        }

        public GenericPool( PoolSettings settings ) : base( settings )
        {
            allLookup = new Dictionary<GameObject, T>();
            inUnused = new Queue<T>();
        }

        private void ReturnObject( GameObject gameObject, T poolable )
        {
            poolable.OnPoolDisable();
            poolable.OnPoolReset();
            outInuse.Remove( gameObject );

            if ( allLookup.Count > settings.Max )
            {
                Debug.Log( "[] Pool under capacity. Destroying new object.");
                Dismantle( poolable ); 
            }
            else
            {
                inUnused.Enqueue( poolable );                
                if ( settings.BackoffProfile != PoolSettings.BackoffProfiles.None && !isBackingOff )
                {
                    StartBackOff();
                }
            }
        }

        private void Dismantle( T poolable )
        {
            allLookup.Remove( poolable.UGameObject );
            poolable.OnWillBeKilled();
            GameObject.Destroy( poolable.UGameObject );
        }

        private void CreateNewObject()
        {
            var newObject = GameObject.Instantiate( settings.Prefab );
            var newT = newObject.GetComponent<T>();
            newT.OnPoolReset();
            newT.OnPoolDisable();

            allLookup.Add( newObject, newT );
            inUnused.Enqueue( newT );
        }

        private void StartBackOff()
        {
            if ( settings.BackoffProfile != PoolSettings.BackoffProfiles.None )
            {
                isBackingOff = true;
                lastBackOffTime = DateTime.Now;
                backoff.Reset();
            }
        }

        private void UpdateBackoff()
        {
            if ( allLookup.Count > settings.BackoffThreshold )
            {
                backoff.UpdateRemaining( allLookup.Count - settings.BackoffThreshold );
                if ( backoff.MoveNext() )
                {
                    int countToDestroy = backoff.Current;
                    DismantleMany( countToDestroy );
                }
            }
        }

        private void DismantleMany( int countToDestroy )
        {            
            while( countToDestroy > 0 && inUnused.Count > settings.BackoffThreshold )
            {
                var next = inUnused.Dequeue();
                Dismantle( next );
            }
        }

        public override K GetPoolable<K>( GameObject clone )
        {
            T poolable;
            if ( allLookup.TryGetValue( clone, out poolable ) )
            {
                return poolable as K;
            }

            throw new PoolException( string.Format( "[Pooler] {0} is not being pooled on this pool({1}).", clone.name, settings.Prefab.name ) );
        }        
    }        
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using nopact.Commons.Pool.Exceptions;
using nopact.Commons.Utility.Timer;
using UnityEngine;

namespace nopact.Commons.Pool
{
    public class PoolProvider :IPoolProvider
    {
        private readonly float providerPoolRefreshTime;
        private readonly PoolBase[ ] poolBuffer;
        private readonly int[ ] prefabBuffer;
        private Dictionary<GameObject, int> objectLookup;
        private ushort nextPoolIndex;
        private bool isBackOffTimerRunning = false;
        private CountdownTimer backoffTimer;
        
        
        public void SetupNewPool<T>( PoolSettings settings ) where T : MonoBehaviour, IPoolable
        {
            if ( nextPoolIndex >= poolBuffer.Length )
            {
                throw new PoolException( "[Pooler] Setup Pool: Max capacity Reached. Increase pooler size." );
            }
            var pool = new GenericPool<T>( settings );
            poolBuffer[ nextPoolIndex ] = pool;
            prefabBuffer[ nextPoolIndex ] = settings.Prefab.GetHashCode();

            if ( !isBackOffTimerRunning )
            {
                backoffTimer = new CountdownTimer( "BackofftimerID", providerPoolRefreshTime, Domain.Enum.CountdownScope.Game, true, ( id ) => OnBackoffTimer( id ), null );
                StartBackoffUpdateTimer();
                isBackOffTimerRunning = true;
            }
            nextPoolIndex++;
        }

        public void PreparePools()
        {
            for ( int index = 0; index < nextPoolIndex; index++ )
            {
                if ( poolBuffer[ index ] != null )
                {
                    poolBuffer[ index ].Prepare();
                }
            }
        }

        public GameObject GetNext<T>( GameObject prefab, out T poolable ) where T : MonoBehaviour, IPoolable
        {
            int hash = prefab.GetHashCode();
            int? poolIndex = FindPool( hash );

            if ( poolIndex == null )
            {
                throw new PoolException( string.Format( "[Pooler] {0} doesn't have a pool.", prefab.name ) );
            }
     
            PoolBase pool = poolBuffer[ poolIndex.Value ];
            var nextObject = pool.Next( out poolable );

            if ( objectLookup == null )
            {
                objectLookup = new Dictionary<GameObject, int>();
            }
            objectLookup.Add( nextObject, poolIndex.Value );
            return nextObject;
        }

        public void Return( GameObject pooledObject , bool redundancyCheck = true)
        {
            int poolIndex;
            if ( !objectLookup.TryGetValue( pooledObject, out poolIndex ) )
            {
                if (redundancyCheck)
                {
                    throw new PoolException("[Pooler] Object doesn't have a pool.");
                }
            }
            else
            {
                var pool = poolBuffer[ poolIndex ];
                objectLookup.Remove( pooledObject );
                pool.Return( pooledObject );
            }
        }
     
        public void ReturnAll( GameObject prefab )
        {
            if ( objectLookup == null )
            {
                return;
            }

            var hash = prefab.GetHashCode();

            int? prefabSlot = null;

            for ( int prefabIndex = 0; prefabIndex < nextPoolIndex; prefabIndex++ )
            {
                if ( prefabBuffer[ prefabIndex ] == hash )
                {
                    prefabSlot = prefabIndex;
                    break;
                }
            }

            if ( prefabSlot == null )
            {
                throw new PoolException( string.Format( "[Pooler] {0} is not pooled, you cannot return all.", prefab.name ) );
            }

            List<GameObject> objectToRemove = new List<GameObject>();
            foreach ( var kvp in objectLookup )
            {
                if ( kvp.Value == prefabSlot.Value )
                {
                    objectToRemove.Add( kvp.Key );
                }
            }

            foreach ( var key in objectToRemove )
            {
                objectLookup.Remove( key );
            }

            poolBuffer[ prefabSlot.Value ].ReturnAll();
        }

        public PoolProvider( ushort capacity, float providerPoolRefreshTime )
        {
            PoolBase.OnPoolableReturn += PoolBaseOnOnPoolableReturn;  
            this.providerPoolRefreshTime = providerPoolRefreshTime;
            poolBuffer = new PoolBase[ capacity ];
            prefabBuffer = new int[ capacity ];
            nextPoolIndex = 0;
        }

        private void PoolBaseOnOnPoolableReturn(IPoolable poolable)
        {
            Return(poolable.UGameObject);
        }

        private int? FindPool( int hash )
        {
            for ( int hashIndex = 0; hashIndex < nextPoolIndex; hashIndex++ )
            {
                if ( prefabBuffer[ hashIndex ] == hash )
                {
                    return hashIndex;
                }
            }

            return null;
        }

        private void StartBackoffUpdateTimer()
        {/*
            backoffTimer.Remaining = providerPoolRefreshTime;
            backoffTimer.Obsolete = false;
            TimerUtility.Instance.RegisterTimer( backoffTimer );*/
        }

        private void OnBackoffTimer( string id )
        {
            for ( int index = 0; index < nextPoolIndex; index++ )
            {
                if ( poolBuffer[ index ] != null )
                {
                    poolBuffer[ index ].RefreshBackoff();
                }
            }
            StartBackoffUpdateTimer();
        }

        public T GetPoolable<T>( GameObject activePooledObject, bool redundancyCheck = true  ) where T : MonoBehaviour, IPoolable
        {
            int poolIndex;
            if ( !objectLookup.TryGetValue( activePooledObject, out poolIndex ) )
            {
                if (redundancyCheck)
                {
                    throw new PoolException( string.Format( "[Pooler] {0} is not pooled, you cannot get its poolable.", activePooledObject.name ) );    
                }
                return null;
            }
     
            var pool = poolBuffer[ poolIndex ];
            return pool.GetPoolable<T>( activePooledObject );
        
        }

        public uint GetActiveCountOf( GameObject prefab )
        {
            try
            {
                return FindPool( prefab ).Count;
            }
            catch ( PoolException e )
            {
                throw new PoolException( string.Format( "[Pooler] {0} is not pooled, you cannot get all active object count.", prefab.name ), e );
            }
        }

        public GameObject[ ] GetAllActive( GameObject prefab )
        {
            try
            {
                return FindPool( prefab ).GetAllActive();
            }
            catch ( PoolException e )
            {
                throw new PoolException( string.Format( "[Pooler] {0} is not pooled, you cannot get all active objects.", prefab.name ), e );
            }
        }

        public T[ ] GetAllActivePoolables<T>( GameObject prefab ) where T : MonoBehaviour, IPoolable
        {
            GameObject[ ] activeObjects = GetAllActive( prefab );
            T[ ] poolables = new T[ activeObjects.Length ];

            for ( int poolableIndex = 0; poolableIndex < poolables.Length; poolableIndex++ )
            {
                poolables[ poolableIndex ] = GetPoolable<T>( activeObjects[ poolableIndex ] );
            }
            return poolables;
        }

        private PoolBase FindPool( GameObject prefab )
        {
            int? slotIndex = null;
            int index = 0;
            for ( ; index < nextPoolIndex; index++ )
            {
                if ( prefabBuffer[ index ] == prefab.GetHashCode() )
                {
                    slotIndex = index;
                    break;
                }
            }

            if ( slotIndex.HasValue )
            {
                return poolBuffer[ slotIndex.Value ];
            }

            throw new PoolException( string.Format( "[Pooler] {0} is not pooled, you cannot get all active objects.", prefab.name ) );
            
        }
    }
}

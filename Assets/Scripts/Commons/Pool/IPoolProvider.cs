using UnityEngine;

namespace nopact.Commons.Pool
{
    public interface IPoolProvider
    {
        GameObject GetNext<T>( GameObject prefab, out T poolable ) where T : MonoBehaviour, IPoolable;
        T GetPoolable<T>( GameObject clone, bool redundancyCheck = true ) where T : MonoBehaviour, IPoolable;
        void Return( GameObject pooledObject , bool redundancyCheck = true);
        void SetupNewPool<T>( PoolSettings settings ) where T : MonoBehaviour, IPoolable;
        void PreparePools();
        GameObject[ ] GetAllActive( GameObject prefab );
        T[ ] GetAllActivePoolables<T>( GameObject prefab ) where T : MonoBehaviour, IPoolable;
        void ReturnAll( GameObject prefab );
        uint GetActiveCountOf( GameObject prefab );
    }
}
using UnityEngine;
using System.Collections;
using System;
using nopact.Game.Resources;

namespace nopact.Commons.SceneDirection.Resources
{
    public interface IResourceProvider
    {
        event Action OnInitializationComplete;
        void Initialize();            
        T Get<T>( string name, out ResourcePair.PoolOptions poolOptions ) where T : UnityEngine.Object;
        GameObject Get(string name, out ResourcePair.PoolOptions poolOptions);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace nopact.Game.Resources
{
    [Serializable]
    public class ResourcePair
    {
        [SerializeField] protected string name;
        [SerializeField] protected UnityEngine.Object asset;
        [SerializeField] protected PoolOptions poolOptions;

        public string Name => name;
        public UnityEngine.Object Object => asset;
        public PoolOptions PoolParams => poolOptions;

        public static IComparer<ResourcePair>Comparer
        {
            get
            {
                return new PairComparer();
            }
        }

        public class PairComparer : IComparer<ResourcePair>
        {
            public int Compare( ResourcePair x, ResourcePair y )
            {
                return string.Compare( x.name, y.name );
            }
        }

        [Serializable]
        public struct PoolOptions
        {
            [SerializeField] private bool isPooled;
            [SerializeField] private ushort poolMax;
            [SerializeField] private ushort poolMin;

            public bool IsPooled => isPooled;
            public ushort PoolMax => poolMax;
            public ushort PoolMin => poolMin;
        }
    }
}

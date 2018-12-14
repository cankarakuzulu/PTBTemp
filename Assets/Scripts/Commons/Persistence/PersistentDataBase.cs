using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nopact.Commons.Persistence
{
    [Serializable]
    public abstract class PersistentDataBase : IPersistentData
    {

        protected Dictionary<string, IDataHolder> data;
        
        public abstract void DataInit();
        public abstract void DataUpdate();        
        public abstract void SetVersion( string version );

        protected T GetDataHolder<T>( string key ) where T : class, IDataHolder, new()
        {
            T item = default( T );
            IDataHolder dataCtx;
            if ( data.TryGetValue( key, out dataCtx ) ) {
                if ( dataCtx != null )
                {
                    item = dataCtx as T;
                    return item;
                }
            }

            item = new T();
            data.Add( key, item );

            return item;
        }

        public virtual string Version
        {
            get;
            protected set;
        }
    }
}

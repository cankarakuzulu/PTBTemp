using nopact.Commons.Utility.Threading;

namespace nopact.Commons.Utility.Singleton
{
    public class Singleton<T> where T : SingletonBase, new()
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
                            instance = new T();
                            instance.Initialize();
                        }
                    }
                }
                return instance;
            }
        }
    }
}

namespace nopact.Commons.Persistence
{
    public interface IPersistence<T> where T : class, IPersistentData, new()
    {
        T Data { get; }

        void Initialize( string version );
        void Reset();
        void Save();
    }
}
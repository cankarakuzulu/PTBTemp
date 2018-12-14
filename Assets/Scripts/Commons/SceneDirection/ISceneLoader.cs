using System;

namespace nopact.Commons.SceneDirection
{
    public interface ISceneLoader
    {

        event Action<string> OnSceneLoaded;
        void LoadLevelScene( string levelName );
        void Unload();
    }
}
using System;

namespace nopact.Commons.SceneDirection
{
    public static class SceneEvents
    {
        public static event Action<string> OnSceneLoaded;

        public static void SceneLoaded( string sceneName )
        {
            if ( OnSceneLoaded != null )
            {
                OnSceneLoaded( sceneName );
            }
        }
    }
}

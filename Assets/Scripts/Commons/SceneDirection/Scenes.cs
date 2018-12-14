//#define MEM_DEBUG
using System;
using nopact.Commons.Domain.Enum;
using nopact.Commons.Utility.Timer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace nopact.Commons.SceneDirection
{
    public class Scenes :MonoBehaviour, ISceneLoader
    {
        [SerializeField]
        protected string initSceneName;
        [SerializeField] protected string mainSceneName, emptySceneName;
        [SerializeField] protected bool loadMenuOnStart;
        [SerializeField] protected float waitingDurationBeforeMenuLoad = 2.0f;

        private event Action<string> SceneLoadedEvent;
        private string expectedLoadingLevelName;
        private CountdownTimer splashAnimationCounter;

        event Action<string> ISceneLoader.OnSceneLoaded
        {
            add
            {
                SceneLoadedEvent += value;
            }

            remove
            {
                SceneLoadedEvent -= value;
            }
        }

        public void LoadUnpersistentScenes()
        {
            if ( loadMenuOnStart )
            {
                TimerUtility.Instance.RegisterTimer( splashAnimationCounter );
            }
        }

        public void LoadLevelScene(string levelName)
        {
            UnloadLevels();
            expectedLoadingLevelName = levelName;
            SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
        }

        public void Unload()
        {
            UnloadLevels();            
        }

        protected void Awake()
        {            
            splashAnimationCounter = new CountdownTimer( "SceneManager_SplashAnimation", waitingDurationBeforeMenuLoad, CountdownScope.Game,
                false, OnSplashAnimationEnded, null );
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;            
        }               

        private void UnloadLevels()
        {
            for (var i = SceneManager.sceneCount - 1; i >= 0; i--)
            {
                var sceneName = SceneManager.GetSceneAt( i ).name;
                if ( sceneName != initSceneName && sceneName != mainSceneName )
                {                    
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                }
            }
        }

        private void UnloadAll()
        {
            for (var i = SceneManager.sceneCount - 1; i >= 0; i--)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
            }
        }

        private void LoadEmpty()
        {
            SceneManager.LoadScene(emptySceneName, LoadSceneMode.Single);
        }

        private void OnSplashAnimationEnded(string timer)
        {
            bool isMainSceneAlreadyLoaded = false;
            for ( var i = SceneManager.sceneCount - 1; i >= 0; i-- )
            {
                var sceneName = SceneManager.GetSceneAt( i ).name;
                if ( sceneName.Equals(mainSceneName) )
                {
                    isMainSceneAlreadyLoaded = true;
                    break;
                }
            }
            if ( isMainSceneAlreadyLoaded )
            {
                SceneEvents.SceneLoaded( mainSceneName );
            }
            else
            {
                SceneManager.LoadScene( mainSceneName, LoadSceneMode.Additive );
            }
        }

       
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.SetActiveScene(scene);
            
            if (scene.name ==mainSceneName)
            {
                expectedLoadingLevelName = string.Empty;
                SceneEvents.SceneLoaded( mainSceneName );
            }
            if ( scene.name == expectedLoadingLevelName )
            {
                SceneEvents.SceneLoaded( expectedLoadingLevelName );
                expectedLoadingLevelName = string.Empty;                
                if ( SceneLoadedEvent != null )
                {
                    SceneLoadedEvent( scene.name );
                }
            }

        }

#if MEM_DEBUG
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(50,50, Screen.width-50, Screen.height / 6 ));
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.yellow;
            if (GUILayout.Button("Load Empty", style, GUILayout.Width(200), GUILayout.Height(60)))
            {
                LoadEmpty();
            }
            //GUILayout.Button("Load Main", style, GUILayout.Width(200), GUILayout.Height(60));
            GUILayout.EndArea();
        }
#endif
        
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log( "[SceneManager] Scene unloaded: " + scene.name );
        }

        public string InitSceneName
        {
            get
            {
                return initSceneName;
            }
        }

        public string MainSceneName
        {
            get
            {
                return mainSceneName;
            }
        }

    }
}

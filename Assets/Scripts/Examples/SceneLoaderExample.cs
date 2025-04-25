using UnityEngine;
using Services;
using Core;
using Cysharp.Threading.Tasks;

namespace Examples
{
    public class SceneLoaderExample : MonoBehaviour
    {
        private ISceneLoader _sceneLoader;

        private void Start()
        {
            _sceneLoader = ServiceLocator.Instance.GetService<ISceneLoader>();
            
            if (_sceneLoader == null)
            {
                MgLogger.LogError("SceneLoader service not found!");
                return;
            }

            _sceneLoader.SceneLoadedEvent += OnSceneLoaded;
            _sceneLoader.SceneUnloadedEvent += OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            if (_sceneLoader != null)
            {
                _sceneLoader.SceneLoadedEvent -= OnSceneLoaded;
                _sceneLoader.SceneUnloadedEvent -= OnSceneUnloaded;
            }
        }
        
        public void LoadMainMenu()
        {
            LoadSceneWithLoadingScreen().Forget();
        }

        private async UniTaskVoid LoadSceneWithLoadingScreen()
        {
            try
            {
                await _sceneLoader.UnloadSceneAsync("MainMenu");
                await _sceneLoader.LoadSceneAsync("RopeDemo");
            }
            catch (System.Exception e)
            {
                MgLogger.LogError($"Failed to load scene: {e.Message}");
            }
        }

        private void OnSceneLoaded(string sceneName)
        {
            MgLogger.Log($"Scene loaded: {sceneName}");
        }

        private void OnSceneUnloaded(string sceneName)
        {
            MgLogger.Log($"Scene unloaded: {sceneName}");
        }
    }
} 
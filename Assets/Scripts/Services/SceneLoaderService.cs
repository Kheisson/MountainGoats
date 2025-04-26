using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core;
using UI;

namespace Services
{
    public class SceneLoaderService : MonoService, ISceneLoader
    {
        private const float MIN_LOADING_TIME = 1f;
        private SceneLoaderUI _loadingScreen;
        private bool _isLoading;
        
        public override bool IsPersistent => true;

        public event Action<string> SceneLoadedEvent;
        public event Action<string> SceneUnloadedEvent;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService<ISceneLoader>(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public async UniTask LoadSceneAsync(string sceneName, bool showLoadingScreen = true)
        {
            if (_isLoading)
            {
                MgLogger.LogWarning($"Already loading a scene. Ignoring request to load {sceneName}");
                return;
            }

            await LoadSceneInternal(sceneName, showLoadingScreen);
        }

        public async UniTask LoadSceneAsync(int sceneIndex, bool showLoadingScreen = true)
        {
            if (_isLoading)
            {
                MgLogger.LogWarning($"Already loading a scene. Ignoring request to load scene index {sceneIndex}");
                return;
            }

            await LoadSceneInternal(sceneIndex, showLoadingScreen);
        }
        
        public async UniTask UnloadSceneAsync(string sceneName, bool showLoadingScreen = true)
        {
            await UnloadSceneInternal(sceneName, showLoadingScreen);
        }
        
        public async UniTask UnloadSceneAsync(int sceneIndex, bool showLoadingScreen = true)
        {
            await UnloadSceneInternal(sceneIndex, showLoadingScreen);
        }
        
        private async UniTask UnloadSceneInternal(object sceneIdentifier, bool showLoadingScreen)
        {
            _isLoading = true;

            if (showLoadingScreen)
            {
                ShowLoadingScreen();
                await _loadingScreen.PlayExitAnimation();
            }

            var loadedScene = SceneManager.GetSceneByName(sceneIdentifier.ToString());

            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                await SceneManager.UnloadSceneAsync(loadedScene);
            }

            if (showLoadingScreen)
            {
                await _loadingScreen.PlayEnterAnimation();
                HideLoadingScreen();
            }

            _isLoading = false;
        }

        private async UniTask LoadSceneInternal(object sceneIdentifier, bool showLoadingScreen)
        {
            _isLoading = true;
            
            var startTime = Time.time;

            if (showLoadingScreen)
            {
                ShowLoadingScreen();
                await _loadingScreen.PlayExitAnimation();
            }
            
            var loadedScene = SceneManager.GetSceneByName(sceneIdentifier.ToString());
            
            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                await SceneManager.UnloadSceneAsync(loadedScene);
            }

            var asyncLoad = sceneIdentifier switch
            {
                string sceneName => SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single),
                int sceneIndex => SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single),
                _ => throw new ArgumentException("Invalid scene identifier type")
            };

            if (asyncLoad != null)
            {
                asyncLoad.allowSceneActivation = false;

                while (asyncLoad.progress < 0.9f)
                {
                    if (_loadingScreen != null)
                    {
                        _loadingScreen.UpdateProgress(asyncLoad.progress);
                    }
                    await UniTask.Yield();
                }

                var elapsedTime = Time.time - startTime;
                
                if (elapsedTime < MIN_LOADING_TIME)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(MIN_LOADING_TIME - elapsedTime));
                }

                asyncLoad.allowSceneActivation = true;
            }

            if (showLoadingScreen)
            {
                await _loadingScreen.PlayEnterAnimation();
                HideLoadingScreen();
            }

            _isLoading = false;
        }

        private void ShowLoadingScreen()
        {
            if (_loadingScreen == null)
            {
                var loadingScreenPrefab = Resources.Load<GameObject>("Prefabs/UI/SceneLoaderUI");
                
                if (loadingScreenPrefab == null)
                {
                    MgLogger.LogError("SceneLoaderUI prefab not found in Resources/Prefabs/UI/");
                    return;
                }
                
                _loadingScreen = Instantiate(loadingScreenPrefab).GetComponent<SceneLoaderUI>();
                DontDestroyOnLoad(_loadingScreen.gameObject);
            }
            _loadingScreen.gameObject.SetActive(true);
        }

        private void HideLoadingScreen()
        {
            if (_loadingScreen != null)
            {
                _loadingScreen.gameObject.SetActive(false);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != ProjectConstants.Scenes.BOOTSTRAPPER)
            {
                SceneLoadedEvent?.Invoke(scene.name);
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (scene.name != ProjectConstants.Scenes.BOOTSTRAPPER)
            {
                SceneUnloadedEvent?.Invoke(scene.name);
            }
        }

        public override void Initialize()
        {
            if (!SceneManager.GetSceneByName(ProjectConstants.Scenes.MAIN_MENU).IsValid())
            {
                SceneManager.LoadScene(ProjectConstants.Scenes.MAIN_MENU, LoadSceneMode.Single);
            }
        }

        public override void Shutdown()
        {
        }
    }
} 
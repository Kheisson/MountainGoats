using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Services;
using Unity.Services.Core;
using UnityEngine;
using Analytics;
using EventsSystem;
using Scenes;
using Storage;

namespace Core
{
    public class BootstrapManager : MonoBehaviour
    {
        [SerializeField] private bool initializeInEditor = false;
        [SerializeField] private SceneLoaderService sceneLoaderService;
        private FullscreenEnforcer fullscreenEnforcer;
        
        private async UniTask Awake()
        {
            if (ServiceLocator.Instance == null)
            {
                MgLogger.LogError("ServiceLocator not found! Make sure it's in the scene.");
                return;
            }

            if (Application.isEditor && !initializeInEditor)
            {
                return;
            }

            await InitializeFullscreen();
            await InitializeServices(); 
            FinishInitialization();
        }

        private void FinishInitialization()
        {
            sceneLoaderService = Instantiate(sceneLoaderService, transform);
            sceneLoaderService.Initialize();
        }

        private async UniTask InitializeFullscreen()
        {
            MgLogger.Log("Initializing fullscreen...");
            
            fullscreenEnforcer = gameObject.GetComponent<FullscreenEnforcer>();
            
            if (fullscreenEnforcer == null)
            {
                fullscreenEnforcer = gameObject.AddComponent<FullscreenEnforcer>();
            }

            fullscreenEnforcer.ForceFullscreen();

            await UniTask.Yield();
            
            MgLogger.Log("Fullscreen initialized successfully");
        }

        private async UniTask InitializeServices()
        {
            MgLogger.Log("Initializing services...");
            
            await InitializeUnityServices();
            
            var dataStorageService = new DataStorageService();
            dataStorageService.Initialize();
            ServiceLocator.Instance.RegisterService<IDataStorageService>(dataStorageService);
            
            var analyticsService = new UnityGameAnalyticsService();
            analyticsService.Initialize();
            ServiceLocator.Instance.RegisterService<IGameAnalyticsService>(analyticsService);
            
            var eventsSystemService = new EventsSystemService();
            eventsSystemService.Initialize();
            ServiceLocator.Instance.RegisterService<IEventsSystemService>(eventsSystemService);
            
            MgLogger.Log("Services initialized successfully");
        }

        private async UniTask InitializeUnityServices()
        {
            var awaiter = UnityServices.InitializeAsync().GetAwaiter();
            
            while (!awaiter.IsCompleted)
            {
                await UniTask.Yield();
            }
            
            MgLogger.Log("Unity services initialized successfully");
            awaiter.GetResult();
        }
    }
} 
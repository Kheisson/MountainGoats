using Cysharp.Threading.Tasks;
using Services;
using Unity.Services.Core;
using UnityEngine;
using Analytics;
using Scenes;
using Storage;

namespace Core
{
    public class BootstrapManager : MonoBehaviour
    {
        [SerializeField] private bool initializeInEditor = false;
        [SerializeField] private SceneLoaderService sceneLoaderService;
        
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

            await InitializeServices();
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
            
            sceneLoaderService = Instantiate(sceneLoaderService, transform);
            sceneLoaderService.Initialize();
            
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
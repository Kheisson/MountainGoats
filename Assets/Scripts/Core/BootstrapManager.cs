using Services;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class BootstrapManager : MonoBehaviour
    {
        [SerializeField] private bool initializeInEditor = false;
        [SerializeField] private SceneLoaderService sceneLoaderService;
        
        private void Awake()
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

            InitializeServices();
        }

        private void InitializeServices()
        {
            MgLogger.Log("Initializing services...");
            
            sceneLoaderService = Instantiate(sceneLoaderService, transform);
            sceneLoaderService.Initialize();
            
            MgLogger.Log("Services initialized successfully");
        }
    }
} 
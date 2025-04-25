using Services;
using UnityEngine;

namespace Core
{
    public class BootstrapManager : MonoBehaviour
    {
        [SerializeField] private bool initializeInEditor = false;
        [SerializeField] private SceneLoader _sceneLoader;
        
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
            
            _sceneLoader = Instantiate(_sceneLoader, transform);
            _sceneLoader.Initialize();
            
            MgLogger.Log("Services initialized successfully");
        }
    }
} 
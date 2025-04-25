using UnityEngine;

namespace MountainGoats.Core
{
    public class BootstrapManager : MonoBehaviour
    {
        [SerializeField] private bool initializeInEditor = false;
        
        private void Awake()
        {
            if (ServiceLocator.Instance == null)
            {
                Debug.LogError("ServiceLocator not found! Make sure it's in the scene.");
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
            Debug.Log("Initializing services...");

            // Create and register pure C# services
            // var scoreService = new ScoreService();
            // ServiceLocator.Instance.RegisterService(scoreService);
            // scoreService.Initialize();

            Debug.Log("Services initialized successfully");
        }
    }
} 
using Services;
using UnityEngine;

namespace Core
{
    public class BootstrapManager : MonoBehaviour
    {
        [SerializeField] private bool initializeInEditor = false;
        
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

        private static void InitializeServices()
        {
            MgLogger.Log("Initializing services...");
            MgLogger.Log("Services initialized successfully");
        }
    }
} 
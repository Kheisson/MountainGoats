using UnityEngine;

namespace MountainGoats.Core
{
    public abstract class MonoService : MonoBehaviour, IService
    {
        protected virtual void Awake()
        {
            var existingService = ServiceLocator.Instance.GetService<MonoService>();
            
            if (existingService != null && existingService != this)
            {
                MGLogger.LogWarning($"Duplicate service of type {GetType().Name} found. Destroying the new instance.", this);
                Destroy(gameObject);
                
                return;
            }
            
            ServiceLocator.Instance.RegisterService(this);
        }

        protected virtual void OnDestroy()
        {
            ServiceLocator.Instance.UnregisterService(this);
        }

        public virtual void Initialize()
        {
            MGLogger.Log("Initializing", this);
        }

        public virtual void Shutdown()
        {
            MGLogger.Log("Shutting down", this);
        }
    }
} 
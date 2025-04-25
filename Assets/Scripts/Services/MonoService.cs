using Core;
using UnityEngine;

namespace Services
{
    public abstract class MonoService : MonoBehaviour, IService
    {
        public bool IsInitialized { get; protected set; }
        
        public virtual void Initialize()
        {
            MgLogger.Log($"Service {GetType().Name} initialized", this);
        }

        public virtual void Shutdown()
        {
            MgLogger.Log($"Service {GetType().Name} shutdown", this);
        }
    }
} 
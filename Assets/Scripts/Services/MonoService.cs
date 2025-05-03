using Core;

namespace Services
{
    public abstract class MonoService : BaseMonoBehaviour, IService
    {
        public bool IsInitialized { get; protected set; }
        public abstract bool IsPersistent { get; }
        
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
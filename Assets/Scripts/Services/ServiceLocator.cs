using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Services
{
    public class ServiceLocator : MonoBehaviour
    {
        private const string SERVICE_LOCATOR_NAME = "ServiceLocator";
        
        private static ServiceLocator _instance;
        
        private readonly Dictionary<Type, IService> _services = new();
        
        public event Action<IService> ServiceRegisteredEvent;
        
        private static bool ApplicationQuitting { get; set; }

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance != null || ApplicationQuitting) return _instance;
                
                var go = new GameObject(SERVICE_LOCATOR_NAME);
                _instance = go.AddComponent<ServiceLocator>();
                DontDestroyOnLoad(go);
                MgLogger.Log("Created new instance", typeof(ServiceLocator));
                
                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                MgLogger.LogWarning("Multiple instances detected. Destroying duplicate.", this);
                Destroy(gameObject);
                
                return;
            }
            
            _instance = this;
            MgLogger.Log("Initialized", this);
        }
        
        private void OnApplicationQuit()
        {
            ApplicationQuitting = true;
        }

        public void RegisterService<T>(T service) where T : class, IService
        {
            RegisterServiceAs<T>(service);
        }

        private void RegisterServiceAs<TInterface>(IService service) where TInterface : class, IService
        {
            if (service == null)
            {
                MgLogger.LogError("Cannot register null service", this);
                return;
            }
            
            var interfaceType = typeof(TInterface);
            
            if (!_services.TryAdd(interfaceType, service))
            {
                MgLogger.LogError($"Service for interface {interfaceType.Name} already registered", this);
                return;
            }

            OnServiceRegistered(service);
            MgLogger.Log($"Registered service as: {interfaceType.Name}", this);
        }

        public void UnregisterService<T>() where T : class, IService
        {
            var serviceType = typeof(T);
            
            if (_services.Remove(serviceType))
            {
                MgLogger.Log($"Unregistered service: {serviceType.Name}", this);
            }
        }

        public T GetService<T>() where T : class, IService
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return service as T;
            }

            foreach (var kvp in _services)
            {
                if (typeof(T).IsAssignableFrom(kvp.Key))
                {
                    return kvp.Value as T;
                }
            }

            MgLogger.LogWarning($"Service of type {typeof(T).Name} not found", this);
            return null;
        }

        private void OnDestroy()
        {
            var localServices = new List<IService>(_services.Values);
            var instanceForLogging = _instance;
            
            _instance = null;
            
            foreach (var service in localServices)
            {
                try 
                {
                    service.Shutdown();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error shutting down service {service.GetType().Name}: {e.Message}");
                }
            }
            
            _services.Clear();
            MgLogger.Log("Shutdown complete", instanceForLogging);
        }

        protected virtual void OnServiceRegistered(IService service)
        {
            ServiceRegisteredEvent?.Invoke(service);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MountainGoats.Core
{
    public class ServiceLocator : MonoBehaviour
    {
        private const string SERVICE_LOCATOR_NAME = "ServiceLocator";
        
        private static ServiceLocator _instance;
        
        private readonly Dictionary<Type, IService> _services = new();

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                var go = new GameObject(SERVICE_LOCATOR_NAME);
                _instance = go.AddComponent<ServiceLocator>();
                DontDestroyOnLoad(go);
                MGLogger.Log("Created new instance", typeof(ServiceLocator));
                
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                MGLogger.LogWarning("Multiple instances detected. Destroying duplicate.", this);
                Destroy(gameObject);
                
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            MGLogger.Log("Initialized", this);
        }

        public void RegisterService(IService service)
        {
            if (service == null)
            {
                MGLogger.LogError("Cannot register null service", this);
                return;
            }

            var serviceType = service.GetType();
            
            if (!_services.TryAdd(serviceType, service))
            {
                MGLogger.LogError($"Service of type {serviceType.Name} already registered", this);
                
                return;
            }

            MGLogger.Log($"Registered service: {serviceType.Name}", this);
        }

        public void UnregisterService(IService service)
        {
            if (service == null) return;

            var serviceType = service.GetType();
            
            if (_services.Remove(serviceType))
            {
                MGLogger.Log($"Unregistered service: {serviceType.Name}", this);
            }
        }

        public T GetService<T>() where T : class, IService
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return service as T;
            }

            MGLogger.LogWarning($"Service of type {typeof(T).Name} not found", this);
            
            return null;
        }

        private void OnDestroy()
        {
            foreach (var service in _services.Values)
            {
                service.Shutdown();
            }
            
            _services.Clear();
            MGLogger.Log("Shutdown complete", this);
        }
    }
} 
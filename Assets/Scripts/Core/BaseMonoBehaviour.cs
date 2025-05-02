using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Services;

namespace Core
{
    public abstract class BaseMonoBehaviour : MonoBehaviour
    {
        protected bool _isInitialized;
        private CancellationTokenSource _cancellationTokenSource;

        protected virtual HashSet<Type> RequiredServices { get; } = new();
        protected virtual void OnServicesInitialized() { }

        protected Camera MainCamera;
        
        protected virtual async void Awake()
        {
            MainCamera = Camera.main;
            _cancellationTokenSource = new CancellationTokenSource();
            await InitializeServicesAsync(_cancellationTokenSource.Token);
        }

        private async UniTask InitializeServicesAsync(CancellationToken cancellationToken)
        {
            while (!_isInitialized && !cancellationToken.IsCancellationRequested)
            {
                var allServicesAvailable = true;
                
                foreach (var serviceType in RequiredServices)
                {
                    if (ServiceLocator.Instance.GetService(serviceType) == null)
                    {
                        allServicesAvailable = false;
                        break;
                    }
                }

                if (allServicesAvailable)
                {
                    OnServicesInitialized();
                    _isInitialized = true;
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }
        protected virtual void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
} 
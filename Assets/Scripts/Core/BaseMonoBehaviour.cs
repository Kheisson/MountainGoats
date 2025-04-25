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
        
        protected abstract HashSet<Type> RequiredServices { get; }

        protected virtual async void Awake()
        {
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

        protected abstract void OnServicesInitialized();

        protected virtual void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
} 
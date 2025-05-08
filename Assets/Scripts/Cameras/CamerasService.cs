using Core;
using Cysharp.Threading.Tasks;
using Services;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Cameras
{
    public class CamerasService : MonoService, ICameraService
    {
        [Header("Cameras")]
        [SerializeField] private CinemachineCamera playerCamera;
        [SerializeField] private CinemachineCamera hookCamera;
    
        [Header("Vignette Settings")]
        [SerializeField] private float vignetteTransitionDuration = 1f;
        [SerializeField] private float maxVignetteIntensity = 0.5f;
    
        private CinemachinePostProcessing _vignetteVolume;
        private float _currentVignetteIntensity;
        private UniTaskCompletionSource<bool> _currentVignetteTask;
        private ECamera _currentCamera;
        
        public override bool IsPersistent => false;

        protected void Awake()
        {
            _vignetteVolume = GetComponentInChildren<CinemachinePostProcessing>();
            
            if (_vignetteVolume == null)
            {
               MgLogger.LogError("Vignette Volume not found!", this);
            }
            
            playerCamera.Priority = 10;
            hookCamera.Priority = 0;
            _currentCamera = ECamera.Player;
            
            ServiceLocator.Instance.RegisterService<ICameraService>(this);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            
            if (ServiceLocator.Instance != null)
            {
                ServiceLocator.Instance.UnregisterService<ICameraService>();
            }
        }

        public async void SwitchCamera(ECamera cameraType, Transform target = null)
        {
            if (_currentCamera == cameraType) return;
            
            _currentVignetteTask?.TrySetCanceled();
            _currentVignetteTask = new UniTaskCompletionSource<bool>();
        
            switch (cameraType)
            {
                case ECamera.Player:
                    playerCamera.Priority = 10;
                    hookCamera.Priority = 0;
                    _currentCamera = ECamera.Player;
                    await TransitionVignetteAsync(false).SuppressCancellationThrow();
                    break;
                
                case ECamera.Hook:
                    if (target != null)
                    {
                        hookCamera.Follow = target;
                    }
                    hookCamera.Priority = 20;
                    playerCamera.Priority = 0;
                    _currentCamera = ECamera.Hook;
                    await TransitionVignetteAsync(true).SuppressCancellationThrow();
                    break;
            }
        }

        private async UniTask TransitionVignetteAsync(bool enable)
        {
            var startIntensity = _currentVignetteIntensity;
            var targetIntensity = enable ? maxVignetteIntensity : 0f;
            var elapsedTime = 0f;

            while (elapsedTime < vignetteTransitionDuration)
            {
                elapsedTime += Time.deltaTime;
                _currentVignetteIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / vignetteTransitionDuration);
                UpdateVignetteIntensity();
                await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }

            _currentVignetteIntensity = targetIntensity;
            UpdateVignetteIntensity();
        }

        private void UpdateVignetteIntensity()
        {
            if (_vignetteVolume != null && _vignetteVolume.Profile.TryGetSettings(out Vignette vignette))
            {
                vignette.intensity.value = _currentVignetteIntensity;
            }
        }
    }
}
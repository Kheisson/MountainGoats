using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace UI
{
    public class SceneLoaderUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _waveImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private RectMask2D _waveMask;
        
        [Header("Animation Settings")]
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _waveRiseDuration = 2f;
        [SerializeField] private float _waveFallDuration = 0.5f;
        [SerializeField] private float _pauseDuration = 1f;
        [SerializeField] private float _pauseProgressThreshold = 0.6f;
        [SerializeField] private Ease _fadeEase = Ease.InOutQuad;
        [SerializeField] private Ease _waveEase = Ease.InOutQuad;
        
        [Header("Wave Shader Settings")]
        [SerializeField] private float _waveSpeed = 1f;
        [SerializeField] private float _waveAmplitude = 0.1f;
        [SerializeField] private float _waveFrequency = 1f;
        
        private Vector2 _originalWavePosition;
        private Sequence _currentSequence;
        private Material _waveMaterial;

        private void Awake()
        {
            _originalWavePosition = _waveImage.rectTransform.anchoredPosition;
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);

            // Create and setup wave material
            _waveMaterial = new Material(Shader.Find("Custom/WaveShader"));
            _waveImage.material = _waveMaterial;
            UpdateWaveShaderProperties();
        }

        private void UpdateWaveShaderProperties()
        {
            if (_waveMaterial != null)
            {
                _waveMaterial.SetFloat("_WaveSpeed", _waveSpeed);
                _waveMaterial.SetFloat("_WaveAmplitude", _waveAmplitude);
                _waveMaterial.SetFloat("_WaveFrequency", _waveFrequency);
            }
        }

        public void UpdateProgress(float progress)
        {
            if (progress >= _pauseProgressThreshold && _currentSequence != null && _currentSequence.IsPlaying())
            {
                _currentSequence.Pause();
                UniTask.Delay(TimeSpan.FromSeconds(_pauseDuration)).ContinueWith(() =>
                {
                    if (_currentSequence != null)
                    {
                        _currentSequence.Play();
                    }
                }).Forget();
            }
        }

        public async UniTask PlayExitAnimation()
        {
            if (_currentSequence != null && _currentSequence.IsPlaying())
            {
                _currentSequence.Kill();
            }

            _currentSequence = DOTween.Sequence();
            _waveImage.rectTransform.anchoredPosition = _originalWavePosition;
            _currentSequence.Append(_canvasGroup.DOFade(1f, _fadeDuration).SetEase(_fadeEase));
            _currentSequence.Join(_waveImage.rectTransform
                .DOAnchorPosY(0f, _waveRiseDuration)
                .SetEase(_waveEase));
            
            await _currentSequence.AsyncWaitForCompletion();
        }

        public async UniTask PlayEnterAnimation()
        {
            if (_currentSequence != null && _currentSequence.IsPlaying())
            {
                _currentSequence.Kill();
            }

            _currentSequence = DOTween.Sequence();
            
            _currentSequence.Append(_waveImage.rectTransform
                .DOAnchorPosY(-_waveImage.rectTransform.rect.height, _waveFallDuration)
                .SetEase(_waveEase));
            
            _currentSequence.Join(_canvasGroup.DOFade(0f, _fadeDuration).SetEase(_fadeEase));
            
            await _currentSequence.AsyncWaitForCompletion();
        }

        private void OnDestroy()
        {
            if (_currentSequence != null)
            {
                _currentSequence.Kill();
            }

            if (_waveMaterial != null)
            {
                Destroy(_waveMaterial);
            }
        }

        private void OnValidate()
        {
            UpdateWaveShaderProperties();
        }
    }
} 
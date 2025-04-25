using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace UI
{
    public class SceneLoaderUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _progressBar;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private Image _backgroundImage;
        
        [Header("Animation Settings")]
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _scaleDuration = 0.5f;
        [SerializeField] private Ease _fadeEase = Ease.InOutQuad;
        [SerializeField] private Ease _scaleEase = Ease.InOutQuad;
        
        private Vector3 _originalScale;
        private Sequence _currentSequence;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _canvasGroup.alpha = 0f;
            _progressBar.fillAmount = 0f;
            _progressText.text = "0%";
            gameObject.SetActive(false);
        }

        public void UpdateProgress(float progress)
        {
            _progressBar.fillAmount = progress;
            _progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }

        public async UniTask PlayExitAnimation()
        {
            if (_currentSequence != null && _currentSequence.IsPlaying())
            {
                _currentSequence.Kill();
            }

            _currentSequence = DOTween.Sequence();
            
            _currentSequence.Append(_canvasGroup.DOFade(1f, _fadeDuration).SetEase(_fadeEase));
            _currentSequence.Join(transform.DOScale(_originalScale, _scaleDuration).SetEase(_scaleEase));
            
            await _currentSequence.AsyncWaitForCompletion();
        }

        public async UniTask PlayEnterAnimation()
        {
            if (_currentSequence != null && _currentSequence.IsPlaying())
            {
                _currentSequence.Kill();
            }

            _currentSequence = DOTween.Sequence();
            
            _currentSequence.Append(_canvasGroup.DOFade(0f, _fadeDuration).SetEase(_fadeEase));
            _currentSequence.Join(transform.DOScale(_originalScale * 1.1f, _scaleDuration).SetEase(_scaleEase));
            
            await _currentSequence.AsyncWaitForCompletion();
        }

        private void OnDestroy()
        {
            if (_currentSequence != null)
            {
                _currentSequence.Kill();
            }
        }
    }
} 
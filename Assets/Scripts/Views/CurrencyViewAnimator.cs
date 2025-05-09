using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Views
{
    public class CurrencyViewAnimator : MonoBehaviour
    {
        [SerializeField] private Image _bigSparkle;
        [SerializeField] private Image _smallSparkle;
        
        [Header("Animation Settings")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _fadeOutDuration = 0.3f;
        [SerializeField] private float _delayBetweenSparkles = 0.2f;
        [SerializeField] private float _delayBeforeReset = 0.5f;
        
        private Sequence _sparkleSequence;
        private Color _bigSparkleColor;
        private Color _smallSparkleColor;
        private bool _isPlaying;

        private void Awake()
        {
            _bigSparkleColor = _bigSparkle.color;
            _smallSparkleColor = _smallSparkle.color;
            
            SetSparklesAlpha(0);
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        public void StartInfiniteAnimation()
        {
            if (_isPlaying) return;
            _isPlaying = true;
            CreateAndPlaySequence();
        }

        public void StopAnimation()
        {
            _isPlaying = false;
            _sparkleSequence?.Kill();
            SetSparklesAlpha(0);
        }

        private void CreateAndPlaySequence()
        {
            if (!_isPlaying) return;

            _sparkleSequence?.Kill();
            _sparkleSequence = DOTween.Sequence();

            _sparkleSequence.Append(_bigSparkle.DOFade(1f, _fadeInDuration).SetUpdate(true))
                           .Join(_smallSparkle.DOFade(1f, _fadeInDuration).SetUpdate(true))
                           .AppendInterval(_delayBetweenSparkles)
                           .Append(_bigSparkle.DOFade(0f, _fadeOutDuration).SetUpdate(true))
                           .Join(_smallSparkle.DOFade(0f, _fadeOutDuration).SetUpdate(true))
                           .AppendInterval(_delayBetweenSparkles);

            _sparkleSequence.Append(_bigSparkle.DOFade(1f, _fadeInDuration).SetUpdate(true))
                           .AppendInterval(_delayBetweenSparkles)
                           .Append(_bigSparkle.DOFade(0f, _fadeOutDuration).SetUpdate(true))
                           .AppendInterval(_delayBetweenSparkles);

            _sparkleSequence.Append(_bigSparkle.DOFade(1f, _fadeInDuration).SetUpdate(true))
                           .Join(_smallSparkle.DOFade(1f, _fadeInDuration).SetUpdate(true))
                           .AppendInterval(_delayBetweenSparkles)
                           .Append(_bigSparkle.DOFade(0f, _fadeOutDuration).SetUpdate(true))
                           .Join(_smallSparkle.DOFade(0f, _fadeOutDuration).SetUpdate(true))
                           .AppendInterval(_delayBeforeReset)
                           .OnComplete(() => {
                               if (_isPlaying)
                               {
                                   CreateAndPlaySequence();
                               }
                               else
                               {
                                   SetSparklesAlpha(0);
                               }
                           });

            _sparkleSequence.SetUpdate(true);
        }

        private void SetSparklesAlpha(float alpha)
        {
            var bigColor = _bigSparkleColor;
            var smallColor = _smallSparkleColor;
            
            bigColor.a = alpha;
            smallColor.a = alpha;
            
            _bigSparkle.color = bigColor;
            _smallSparkle.color = smallColor;
        }
    }
} 
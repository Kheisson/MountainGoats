using UnityEngine;
using TMPro;
using Controllers;
using DG.Tweening;

namespace Views
{
    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currencyText;
        [SerializeField] private CurrencyViewAnimator _animator;

        private CurrencyController _controller;

        public void Initialize(CurrencyController controller)
        {
            _controller = controller;
            _controller.OnCurrencyChanged += UpdateCurrencyDisplay;
            UpdateCurrencyDisplay(_controller.GetCurrency());
            _animator?.StartInfiniteAnimation();
        }

        private void OnDestroy()
        {
            if (_controller != null)
            {
                _controller.OnCurrencyChanged -= UpdateCurrencyDisplay;
            }
            _animator?.StopAnimation();
        }

        private void UpdateCurrencyDisplay(int newAmount)
        {
            if (_currencyText != null)
            {
                _currencyText.text = newAmount.ToString();
            }
        }

        public void PlayNotEnoughFundsAnimation()
        {
            if (_currencyText == null) return;

            _currencyText.transform.DOKill();

            var sequence = DOTween.Sequence();

            sequence.Append(_currencyText.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f))
                    .OnKill(() => _currencyText.transform.localScale = Vector3.one)
                    .SetUpdate(true);
        }
    }
} 
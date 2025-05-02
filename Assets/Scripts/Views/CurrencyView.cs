using UnityEngine;
using TMPro;
using Controllers;

namespace Views
{
    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currencyText;

        private CurrencyController _controller;

        public void Initialize(CurrencyController controller)
        {
            _controller = controller;
            _controller.OnCurrencyChanged += UpdateCurrencyDisplay;
            UpdateCurrencyDisplay(_controller.GetCurrency());
        }

        private void OnDestroy()
        {
            if (_controller != null)
            {
                _controller.OnCurrencyChanged -= UpdateCurrencyDisplay;
            }
        }

        private void UpdateCurrencyDisplay(int newAmount)
        {
            if (_currencyText != null)
            {
                _currencyText.text = newAmount.ToString();
            }
        }
    }
} 
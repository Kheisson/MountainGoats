using System;
using Core;

namespace Models
{
    public class CurrencyModel
    {
        private int _currency;
        
        public int Currency
        {
            get => _currency;
            
            private set
            {
                if (_currency != value)
                {
                    _currency = value;
                    OnCurrencyChanged?.Invoke(_currency);
                }
            }
        }

        public event Action<int> OnCurrencyChanged;

        public CurrencyModel(int initialAmount = 0)
        {
            _currency = initialAmount;
        }

        public bool HasSufficientFunds(int amount)
        {
            return _currency >= amount;
        }

        public bool AddCurrency(int amount)
        {
            if (amount < 0)
            {
                MgLogger.LogWarning("Attempted to add negative currency amount");
                return false;
            }

            Currency += amount;
            return true;
        }

        public bool SubtractCurrency(int amount)
        {
            if (amount < 0)
            {
                MgLogger.LogWarning("Attempted to subtract negative currency amount");
                return false;
            }

            if (!HasSufficientFunds(amount))
            {
                MgLogger.LogWarning("Insufficient funds for subtraction");
                return false;
            }

            Currency -= amount;
            return true;
        }
    }
} 
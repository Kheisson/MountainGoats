using System;
using Core;
using EventsSystem;
using GarbageManagement;
using Models;
using Storage;
using UnityEngine;
using Views;
using TMPro;
using DG.Tweening;

namespace Controllers
{
    public class CurrencyController
    {
        private readonly CurrencyModel _model;
        private readonly IDataStorageService _dataStorage;
        private readonly IEventsSystemService _eventsSystemService;
        private readonly CurrencyView _currencyView;

        public event Action<int> OnCurrencyChanged
        {
            add => _model.OnCurrencyChanged += value;
            remove => _model.OnCurrencyChanged -= value;
        }

        public CurrencyController(IDataStorageService dataStorage, CurrencyView view,
            IEventsSystemService eventsSystemService)
        {
            _dataStorage = dataStorage;
            _model = new CurrencyModel(_dataStorage.GetGameData().currency);
            _model.OnCurrencyChanged += PersistCurrency;
            _eventsSystemService = eventsSystemService;
            _eventsSystemService.Subscribe<GarbageCollectedData>(ProjectConstants.Events.GARBAGE_COLLECTED, OnGarbageCollected);
            _currencyView = view;
            _currencyView.Initialize(this);
        }

        public int GetCurrency()
        {
            return _model.Currency;
        }

        public bool HasSufficientFunds(int amount)
        {
            return _model.HasSufficientFunds(amount);
        }

        public bool AddCurrency(int amount)
        {
            var success = _model.AddCurrency(amount);
            
            if (success)
            {
                MgLogger.Log($"Added {amount} currency. New balance: {_model.Currency}");
            }
            
            return success;
        }

        public bool SubtractCurrency(int amount)
        {
            var success = _model.SubtractCurrency(amount);
            
            if (success)
            {
                MgLogger.Log($"Subtracted {amount} currency. New balance: {_model.Currency}");
            }
            
            return success;
        }
        
        private void OnGarbageCollected(GarbageCollectedData obj)
        {
            var (weight, garbageValue) = obj.Garbage.ItemData.CalculateRandomValue();
            
            AddCurrency(Mathf.RoundToInt(garbageValue));
        }

        private void PersistCurrency(int newAmount)
        {
            _dataStorage.ModifyGameDataSync(gameData =>
            {
                gameData.currency = newAmount;
                return true;
            });
        }

        public void DisplayNotEnoughFunds()
        {
            _currencyView?.PlayNotEnoughFundsAnimation();
        }
    }
} 
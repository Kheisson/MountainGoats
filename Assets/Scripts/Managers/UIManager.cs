using System;
using System.Collections.Generic;
using Controllers;
using Core;
using EventsSystem;
using Services;
using Storage;
using UnityEngine;
using Views;
using Views.Shop;

namespace Managers
{
    public class UIManager : MonoService, IUiManager
    {
        [SerializeField] private Canvas _hudCanvas;
        [SerializeField] private CanvasGroup _hudCanvasGroup;
        [SerializeField] private CurrencyView _viewPrefab;

        private CurrencyView _currencyView;
        private ShopView _showView;
        
        private IDataStorageService _dataStorage;
        private IEventsSystemService _eventsSystemService;
        
        public CurrencyController CurrencyController { get; private set; }
        
        protected override HashSet<Type> RequiredServices => new()
        {
            typeof(IDataStorageService),
            typeof(IEventsSystemService),
        };
        
        public override bool IsPersistent => false;
        
        protected override void OnServicesInitialized()
        {
            _eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
            _dataStorage = ServiceLocator.Instance.GetService<IDataStorageService>();
            _currencyView = Instantiate(_viewPrefab, _hudCanvas.transform);
            CurrencyController = new CurrencyController(_dataStorage, _currencyView, _eventsSystemService);
            ServiceLocator.Instance.RegisterService<IUiManager>(this);
        }
        
        public void ShowHudCanvas()
        {
            if (_hudCanvasGroup == null)
            {
                MgLogger.LogError("HudCanvasGroup is not assigned.");
                return;
            }
            
            _hudCanvasGroup.alpha = 1f;
            _hudCanvasGroup.interactable = true;
            _hudCanvasGroup.blocksRaycasts = true;
        }
        
        public void HideHudCanvas()
        {
            if (_hudCanvasGroup == null)
            {
                MgLogger.LogError("HudCanvasGroup is not assigned.");
                return;
            }
            
            _hudCanvasGroup.alpha = 0f;
            _hudCanvasGroup.interactable = false;
            _hudCanvasGroup.blocksRaycasts = false;
        }
    }
} 
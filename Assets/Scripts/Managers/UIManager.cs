using Controllers;
using Core;
using Services;
using Storage;
using UnityEngine;
using Views;

namespace Managers
{
    public class UIManager : MonoService, IUiManager
    {
        [SerializeField] private Canvas _hudCanvas;
        [SerializeField] private CanvasGroup _hudCanvasGroup;
        [SerializeField] private CurrencyView _viewPrefab;
        [SerializeField] private ShopView _shopViewPrefab;
        
        private IDataStorageService _dataStorage;
        
        public CurrencyController CurrencyController { get; private set; }
        public override bool IsPersistent => false;

        private void Start()
        {
            _viewPrefab = Instantiate(_viewPrefab, _hudCanvas.transform);
            _dataStorage = ServiceLocator.Instance.GetService<IDataStorageService>();
            CurrencyController = new CurrencyController(_dataStorage, _viewPrefab);
            
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
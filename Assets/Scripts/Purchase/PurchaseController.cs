using System;
using System.Collections.Generic;
using Controllers;
using Core;
using EventsSystem;
using Managers;
using Services;
using Storage;
using Upgrades;
using Views.Shop;

namespace Purchase
{
    public class PurchaseController : BaseMonoBehaviour, IDisposable
    {
        private CurrencyController _currencyController;
        private IEventsSystemService _eventsSystemService;
        private IDisposable _purchaseClickSubscription;
        private IDataStorageService _dataStorageService;

        protected override HashSet<Type> RequiredServices => new HashSet<Type>()
        {
            typeof(IUiManager),
            typeof(IEventsSystemService)
        };

        protected override void OnServicesInitialized()
        {
            _currencyController = ServiceLocator.Instance.GetService<IUiManager>().CurrencyController;
            _eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
            _dataStorageService = ServiceLocator.Instance.GetService<IDataStorageService>();

            _purchaseClickSubscription = _eventsSystemService.Subscribe<PurchaseButtonClickedEvent>(ProjectConstants.Events.PURCHASE_BUTTON_CLICKED, TryPurchase);
        }

        private void TryPurchase(PurchaseButtonClickedEvent eventData)
        {
            if (_currencyController.HasSufficientFunds(eventData.UpgradePath.AvailableUpgrades[eventData.UpgradeIndex]
                    .Cost))
            {
                _dataStorageService.ModifyGameDataSync(data => UpdateUpgradeDataOnModel(data, eventData.UpgradeType, eventData.UpgradeIndex));
                _eventsSystemService.Publish(ProjectConstants.Events.UPGRADE_PURCHASED, _dataStorageService.GetGameData().upgradesModel);
            }
            else
            {
                _currencyController.DisplayNotEnoughFunds();
            }
        }

        private bool UpdateUpgradeDataOnModel(GameData data, EUpgradeType upgradeType, int upgradeIndex)
        {
            data.upgradesModel.PurchaseUpgrade(upgradeType, upgradeIndex);
            return true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Dispose();
        }

        public void Dispose()
        {
            _purchaseClickSubscription?.Dispose();
        }
    }
}
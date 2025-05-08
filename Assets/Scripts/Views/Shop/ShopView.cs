using System;
using System.Collections.Generic;
using Core;
using EventsSystem;
using Models;
using Services;
using Storage;
using UnityEngine;
using Upgrades;

namespace Views.Shop
{
    public class ShopView : BaseMonoBehaviour
    {
        [SerializeField] private Transform upgradesHolder;
        [SerializeField] private UpgradePathView upgradePathPrefab;
        [SerializeField] private UpgradePathsTable upgradePathsTable;

        private readonly Dictionary<EUpgradeType, UpgradePathView> _upgradePathViews = new ();
        private UpgradesModel _upgradesModel;
        private IDisposable _refreshViewSubscription;

        protected override HashSet<Type> RequiredServices => new HashSet<Type>()
        {
            typeof(IDataStorageService),
            typeof(IEventsSystemService)
        };
        
        protected override void OnServicesInitialized()
        {
            var upgradesModel = ServiceLocator.Instance.GetService<IDataStorageService>().GetGameData().upgradesModel;
            _refreshViewSubscription = ServiceLocator.Instance.GetService<IEventsSystemService>()
                .Subscribe<UpgradesModel>(ProjectConstants.Events.UPGRADE_PURCHASED, RefreshView);
            
            RefreshView(upgradesModel);
        }

        private void RefreshView(UpgradesModel upgradesModel)
        {
            _upgradesModel = upgradesModel;
            
            foreach (var upgradeType in upgradePathsTable.UpgradePaths.Keys)
            {
                SetupUpgradeView(upgradeType);
            }
        }

        private void SetupUpgradeView(EUpgradeType upgradeType)
        {
            var upgradePath = upgradePathsTable.UpgradePaths[upgradeType];
            var availableUpgrades = upgradePath.AvailableUpgrades;

            int maxIndex;
            
            if (_upgradesModel.TryGetUpgradesByType(upgradeType, out var purchasedIndex))
            {
                maxIndex = purchasedIndex;
            }
            else
            {
                maxIndex = -1;
            }

            if (!_upgradePathViews.ContainsKey(upgradeType))
            {
                var instantiatedUpgradePath = Instantiate(upgradePathPrefab, upgradesHolder);
                _upgradePathViews[upgradeType] = instantiatedUpgradePath;
            }
            
            _upgradePathViews[upgradeType].UpdateView(availableUpgrades, maxIndex, upgradePath, upgradeType);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _refreshViewSubscription?.Dispose();
        }
    }
}
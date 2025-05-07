using System;
using System.Collections.Generic;
using System.Linq;
using Core;
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

        protected override HashSet<Type> RequiredServices => new HashSet<Type>()
        {
            typeof(IDataStorageService)
        };
        
        protected override void OnServicesInitialized()
        {
            _upgradesModel = ServiceLocator.Instance.GetService<IDataStorageService>().GetGameData().upgradesModel;
            
            foreach (var upgradeType in upgradePathsTable.UpgradePaths.Keys)
            {
                SetupUpgradeView(upgradeType);
            }
        }

        private UpgradesModel _upgradesModel;
        
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
            
            var instantiatedUpgradePath = Instantiate(upgradePathPrefab, upgradesHolder);
            instantiatedUpgradePath.UpdateView(availableUpgrades, maxIndex, upgradePath, upgradeType);
            _upgradePathViews[upgradeType] = instantiatedUpgradePath;
        }
    }
}
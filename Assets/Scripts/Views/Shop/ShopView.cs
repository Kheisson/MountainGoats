using System.Collections.Generic;
using System.Linq;
using Models;
using UnityEngine;
using Upgrades;

namespace Views.Shop
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private Transform upgradeHolders;
        [SerializeField] private UpgradePathView upgradePathPrefab;
        [SerializeField] private GameObject upgradeButton;
        [SerializeField] private UpgradePathsTable upgradePathsTable;

        private Dictionary<EUpgradeType, UpgradePathView> _upgradePathViews;

        private UpgradesModel _upgradesModel;

        private void Start()
        {
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
            
            var instantiatedUpgradePath = Instantiate(upgradePathPrefab, upgradeHolders);
            instantiatedUpgradePath.UpdateView(availableUpgrades, maxIndex, upgradePath, upgradeType);
            _upgradePathViews[upgradeType] = instantiatedUpgradePath;
        }
    }
}
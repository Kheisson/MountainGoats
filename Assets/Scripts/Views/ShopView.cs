using System.Linq;
using Models;
using UnityEngine;
using Upgrades;

namespace Views
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private Transform upgradeHolders;
        [SerializeField] private GameObject upgradePathPrefab;
        [SerializeField] private UpgradePathsTable upgradePathsTable;

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

            bool hasPurchasedUpgrades = (_upgradesModel.TryGetUpgradesByType(upgradeType, out var purchasedIndices));

            for (int i = 0; i < availableUpgrades.Count; i++)
            {
                bool isPurchased = hasPurchasedUpgrades && purchasedIndices.Contains(i);
                bool isLocked = upgradePath.isLinearUpgrade &&
                                (
                                    (hasPurchasedUpgrades && i > purchasedIndices.Max()) ||
                                    (i > 0)
                                );
                                
                                
                EButtonDisplayState displayState = isPurchased ? EButtonDisplayState.Purchased :
                    isLocked ? EButtonDisplayState.Locked : EButtonDisplayState.Unlocked;

                SetupUpgradeButton(availableUpgrades[i], displayState);
            }
        }

        private void SetupUpgradeButton(UpgradeData availableUpgrade, EButtonDisplayState displayState)
        {
            
        }
    }
}
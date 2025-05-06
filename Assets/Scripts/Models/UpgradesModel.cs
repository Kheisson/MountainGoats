using System.Collections.Generic;
using Upgrades;

namespace Models
{
    public class UpgradesModel
    {
        private Dictionary<EUpgradeType, int> _purchasedUpgrades = new ();

        public bool TryGetUpgradesByType(EUpgradeType upgradeType, out int maxPurchasedIndex)
        {
            return _purchasedUpgrades.TryGetValue(upgradeType, out maxPurchasedIndex);
        }

        public void PurchaseUpgrade(EUpgradeType upgradeType, int upgradeIndex)
        {
            _purchasedUpgrades[upgradeType] = upgradeIndex;
        }
    }
}
using System.Collections.Generic;
using Upgrades;

namespace Models
{
    public class UpgradesModel
    {
        private Dictionary<EUpgradeType, List<int>> _purchasedUpgrades = new ();

        public bool TryGetUpgradesByType(EUpgradeType upgradeType, out List<int> upgrades)
        {
            return _purchasedUpgrades.TryGetValue(upgradeType, out upgrades);
        }

        public void PurchaseUpgrade(EUpgradeType upgradeType, int upgradeIndex)
        {
            if (_purchasedUpgrades.ContainsKey(upgradeType))
            {
                _purchasedUpgrades[upgradeType].Add(upgradeIndex);
            }
            else
            {
                _purchasedUpgrades[upgradeType] = new List<int>(){ upgradeIndex };
            }
        }
    }
}
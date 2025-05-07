using System.Collections.Generic;
using Upgrades;
using System.Linq;
namespace Models
{
    public class UpgradesModel
    {
        private Dictionary<EUpgradeType, int> _purchasedUpgrades = new ();

        public UpgradesModel(UpgradesModel upgradesModel)
        {
            foreach (var kvp in upgradesModel._purchasedUpgrades)
            {
                _purchasedUpgrades[kvp.Key] = kvp.Value;
            }
        }

        public UpgradesModel()
        {
            
        }

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
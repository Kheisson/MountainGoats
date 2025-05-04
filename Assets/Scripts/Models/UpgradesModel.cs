using System.Collections.Generic;
using Upgrades;

namespace Models
{
    public class UpgradesModel
    {
        private Dictionary<EUpgradeType, List<int>> _purchasedUpgrades = new ();

        public void PurchaseUpgrade(EUpgradeType upgradeType, int upgradeIndex)
        {
            _purchasedUpgrades[upgradeType].Add(upgradeIndex);
        }
        
    }
}
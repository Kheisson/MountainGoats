using Upgrades;

namespace Views.Shop
{
    public class PurchaseButtonClickedEvent
    {
        public UpgradePath UpgradePath { get; private set; }
        public int UpgradeIndex { get; private set; }
        public EUpgradeType UpgradeType { get; private set; }

        public PurchaseButtonClickedEvent(UpgradePath upgradePath, int upgradeIndex, EUpgradeType upgradeType)
        {
            UpgradePath = upgradePath;
            UpgradeIndex = upgradeIndex;
            UpgradeType = upgradeType;
        }

    }
}
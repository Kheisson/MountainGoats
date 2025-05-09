using Upgrades;

namespace Events
{
    public class UpgradeHoverEvent
    {
        public UpgradeData UpgradeData { get; }
        public bool IsPurchased { get; }

        public UpgradeHoverEvent(UpgradeData upgradeData, bool isPurchased)
        {
            UpgradeData = upgradeData;
            IsPurchased = isPurchased;
        }
    }
} 
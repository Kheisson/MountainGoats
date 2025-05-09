using Core;
using UnityEngine;
using UnityEngine.UI;
using Upgrades;

namespace Views.Shop
{
    public class UpgradeIconView : BaseMonoBehaviour
    { 
        [SerializeField] private Image image;
        
        private UpgradeData _upgradeData;

        public void SetupIcon(UpgradePath upgradePath, int purchasedIndex, bool isPurchased = false)
        {
            _upgradeData = upgradePath.AvailableUpgrades[purchasedIndex];
            image.sprite = isPurchased ? _upgradeData.PostPurchaseIcon : _upgradeData.PrePurchaseIcon;
        }
    }
}
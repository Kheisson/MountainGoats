using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Upgrades;

namespace Views.Shop
{
    public class ShopDescriptionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Image iconImage;
        
        public void Display(UpgradeData upgradeData)
        {
            SetComponentActive(true);
            
            descriptionText.text = upgradeData.Description;
            costText.text = $"Cost: {upgradeData.Cost}$";
            iconImage.sprite = upgradeData.Icon;
        }

        public void Hide()
        {
            SetComponentActive(false);
        }

        private void SetComponentActive(bool isEnabled)
        {
            descriptionText.enabled = isEnabled;
            costText.enabled = isEnabled;
            iconImage.enabled = isEnabled;
        }
    }
}
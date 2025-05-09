using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using Upgrades;

namespace Views.Shop
{
    public class UpgradePathView : MonoBehaviour
    {
        [SerializeField] private GameObject noUpgradesPrefab;
        [SerializeField] private GameObject maxUpgradesPrefab;
        [SerializeField] private UpgradeIconView upgradeIconViewPrefab;

        [SerializeField] private Transform leftHolder;
        [SerializeField] private Transform rightHolder;

        [SerializeField] private UpgradeButton upgradeButton;
        [SerializeField] private TextMeshProUGUI header;

        [SerializeField] private SerializedDictionary<EUpgradeType, string> headersByType;
            
        public void UpdateView(List<UpgradeData> availableUpgrades, int maxPurchasedIndex, UpgradePath upgradePath, EUpgradeType upgradeType)
        {
            bool hasPurchasedUpgrades = maxPurchasedIndex >= 0;
            bool purchasedMaxUpgrades = maxPurchasedIndex == availableUpgrades.Count - 1;
            header.text = headersByType[upgradeType];

            if (!hasPurchasedUpgrades)
            {
                InstantiateInHolder(noUpgradesPrefab, leftHolder);
                var upgradableIcon = InstantiateInHolder(upgradeIconViewPrefab, rightHolder);
                upgradableIcon.SetupIcon(upgradePath, maxPurchasedIndex + 1);
                upgradeButton.SetupButton(maxPurchasedIndex + 1, upgradePath, upgradeType);
            }
            else if (purchasedMaxUpgrades)
            {
                InstantiateInHolder(maxUpgradesPrefab, rightHolder);
                var upgradedIcon = InstantiateInHolder(upgradeIconViewPrefab, leftHolder);
                upgradedIcon.SetupIcon(upgradePath, maxPurchasedIndex);
                upgradeButton.gameObject.SetActive(false);
            }
            else
            {
                var upgradedIcon = InstantiateInHolder(upgradeIconViewPrefab, leftHolder);
                upgradedIcon.SetupIcon(upgradePath, maxPurchasedIndex);
                
                var upgradableIcon = InstantiateInHolder(upgradeIconViewPrefab, rightHolder);
                upgradableIcon.SetupIcon(upgradePath, maxPurchasedIndex + 1);
                
                upgradeButton.SetupButton(maxPurchasedIndex + 1, upgradePath, upgradeType);
            }
        }

        private TObject InstantiateInHolder<TObject>(TObject prefab, Transform parent) where TObject : Object
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }

            return Instantiate(prefab, parent);
        }
    }
}
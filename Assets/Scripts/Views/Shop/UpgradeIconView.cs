using System;
using System.Collections.Generic;
using Core;
using EventsSystem;
using Services;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Upgrades;

namespace Views.Shop
{
    public class UpgradeIconView : BaseMonoBehaviour
    { 
        [SerializeField] private Image image;

        public void SetupIcon(UpgradePath upgradePath, int purchasedIndex)
        {
            image.sprite = upgradePath.AvailableUpgrades[purchasedIndex].Icon;
        }
    }
}
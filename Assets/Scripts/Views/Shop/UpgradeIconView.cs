using System;
using System.Collections.Generic;
using Core;
using EventsSystem;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Upgrades;

namespace Views.Shop
{
    public class UpgradeIconView : BaseMonoBehaviour
    { 
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void SetupIcon(UpgradePath upgradePath, int purchasedIndex)
        {
            spriteRenderer.sprite = upgradePath.AvailableUpgrades[purchasedIndex].Icon;
        }
    }
}
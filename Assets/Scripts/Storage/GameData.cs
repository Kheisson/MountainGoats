using System;
using System.Collections.Generic;
using Models;
using Upgrades;

namespace Storage
{
    [Serializable]
    public class GameData
    {
        public int depthReached; 
        public int currency = 0;
        public Dictionary<EUpgradeType, int> purchasedUpgrades = new();
        public string lastSelectedItemId;
        public List<string> unlockedItems = new();
        public int lastSelectedTabIndex = 0;

        public GameData Copy()
        {
            return new GameData
            {
                depthReached = depthReached,
                currency = currency,
                unlockedItems = new List<string>(unlockedItems),        
                lastSelectedItemId = lastSelectedItemId,
                purchasedUpgrades = purchasedUpgrades,
                lastSelectedTabIndex = lastSelectedTabIndex
            };
        }
    }
}
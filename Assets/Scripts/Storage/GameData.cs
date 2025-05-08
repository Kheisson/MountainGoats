using System;
using System.Collections.Generic;
using Models;

namespace Storage
{
    [Serializable]
    public class GameData
    {
        public int depthReached; 
        public int currency = 0;
        public UpgradesModel upgradesModel = new();
        public string lastSelectedItemId;
        public List<string> unlockedItems = new();

        public GameData Copy()
        {
            return new GameData
            {
                depthReached = depthReached,
                currency = currency,
                unlockedItems = new List<string>(unlockedItems),        
                lastSelectedItemId = lastSelectedItemId,
                upgradesModel = new UpgradesModel(upgradesModel),
            };
        }
    }
}
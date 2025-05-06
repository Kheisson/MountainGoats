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
        public UpgradesModel upgradesModel;
        public HashSet<string> _unlockedItems = new();
        public string lastSelectedItemId;

        public GameData Copy()
        {
            return new GameData
            {
                depthReached = depthReached,
                currency = currency,
                _unlockedItems = new HashSet<string>(_unlockedItems),
                lastSelectedItemId = lastSelectedItemId,
            };
        }
    }
}
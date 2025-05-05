using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Storage
{
    [Serializable]
    public class GameData
    {
        [FormerlySerializedAs("DepthReached")] public int depthReached;
        [FormerlySerializedAs("Currency")] public int currency = 0;
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
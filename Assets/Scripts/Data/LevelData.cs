using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class LevelData
    {
        [field: SerializeField] 
        public List<GarbageItemData> SpawnableItems { get; private set; }

        [field: SerializeField] 
        public int MinItemsForLevel { get; private set; } = 1;

        [field: SerializeField] 
        public int MaxItemsForLevel { get; private set; } = 4;
        
        [field: SerializeField] 
        public float HorizontalRange { get; private set; } = 4;
    }
}
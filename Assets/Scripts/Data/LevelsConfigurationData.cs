using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Data/Level Configuration")]
    public class LevelsConfigurationData : ScriptableObject
    {
        [field: SerializeField] 
        public float DepthPerLevel { get; private set; } = 5f;

        [field:SerializeField] 
        public List<LevelData> Levels { get; private set; }
    }
}
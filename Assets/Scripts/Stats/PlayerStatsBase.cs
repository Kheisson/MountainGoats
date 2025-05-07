using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(menuName = "Data/Player base stats")]
    public class PlayerStatsBase : ScriptableObject, IPlayerStats
    {
        [field: SerializeField]
        public float RopeLength { get; private set; }
        
        [field: SerializeField]
        public float HorizontalSpeed { get; private set; }
        
        [field: SerializeField]
        public float LightIntensity { get; private set; }
        
        [field: SerializeField]
        public float EnergyCapacity { get; private set; }
    }
}
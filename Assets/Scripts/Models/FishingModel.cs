using UnityEngine;

namespace Models
{
    public class FishingModel : ScriptableObject, IFishingModel
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
using GarbageManagement;
using UnityEngine;

namespace Data
{
    public class GarbageItemData : ScriptableObject
    {
        [Header("Visuals")]
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private Garbage itemPrefab;

        [Header("Item Information")]
        [SerializeField] private string itemName;
        [TextArea(3, 5)]
        [SerializeField] private string description;

        [Header("Weight Properties")]
        [SerializeField] private float minWeight = 0.1f;
        [SerializeField] private float maxWeight = 1.0f;

        [Header("Currency Settings")]
        [SerializeField] private float baseValue = 1.0f;
        [SerializeField] private float weightMultiplier = 1.0f;

        public Sprite ItemSprite => itemSprite;
        public Garbage ItemPrefab => itemPrefab;
        public string ItemName => itemName;
        public string Description => description;
        public float MinWeight => minWeight;
        public float MaxWeight => maxWeight;
        
        private float GetRandomWeight()
        {
            return Random.Range(minWeight, maxWeight);
        }
        
        private float CalculateValue(float weight)
        {
            return baseValue + (weight * weightMultiplier);
        }
        
        public (float weight, float value) CalculateRandomValue()
        {
            var weight = GetRandomWeight();
            
            return (weight, CalculateValue(weight));
        }
    }
} 
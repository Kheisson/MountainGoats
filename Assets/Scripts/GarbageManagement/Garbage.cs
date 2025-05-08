using Core;
using Data;
using EventsSystem;
using UnityEngine;

namespace GarbageManagement
{
    public class Garbage : BaseMonoBehaviour
    {
        private float _weight;
        private float _value;
        private int _levelIndex;
        private IEventsSystemService _eventsSystemService;

        public GarbageItemData ItemData { get; private set; }

        public void Initialize(int levelIndex, float weight, float value, IEventsSystemService eventsSystemService, GarbageItemData itemData)
        {
            _levelIndex = levelIndex;
            _weight = weight;
            _value = value;
            _eventsSystemService = eventsSystemService;
            ItemData = itemData;
        }

        private void OnGarbageCollected()
        {
            _eventsSystemService?.Publish(ProjectConstants.Events.GARBAGE_COLLECTED, GenerateGarbageCollectedData());
        }
        
        private void OnGarbageHooked()
        {
            _eventsSystemService?.Publish(ProjectConstants.Events.GARBAGE_HOOKED, GenerateGarbageHookedData());
        }

        private GarbageCollectedData GenerateGarbageCollectedData()
        {
            return new GarbageCollectedData(_levelIndex, _weight, _value, this);
        }
        
        private GarbageHookedData GenerateGarbageHookedData()
        {
            return new GarbageHookedData(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnGarbageHooked();
        }

        public void Collect()
        {
            OnGarbageCollected();
            Destroy(gameObject);
        }
    }
}


using System;
using System.Collections.Generic;
using Core;
using EventsSystem;

namespace GarbageManagement
{
    public class Garbage : BaseMonoBehaviour
    {
        private float _weight;
        private float _value;
        private int _levelIndex;
        private IEventsSystemService _eventsSystemService;

        protected override HashSet<Type> RequiredServices => new HashSet<Type>()
        {
            typeof(IEventsSystemService)
        };

        public void Initialize(int levelIndex, float weight, float value)
        {
            _levelIndex = levelIndex;
            _weight = weight;
            _value = value;
        
            transform.localScale *= weight;
        }

        private void OnGarbageCollected()
        {
            _eventsSystemService?.Publish(ProjectConstants.Events.GARBAGE_COLLECTED, GenerateGarbageCollectedData());
        }

        private GarbageCollectedData GenerateGarbageCollectedData()
        {
            return new GarbageCollectedData(_levelIndex, _weight, _value);
        }
    }
}


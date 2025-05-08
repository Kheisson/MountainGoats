using Core;
using Data;
using DG.Tweening;
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
        private Sequence _floatingSequence;

        public GarbageItemData ItemData { get; private set; }

        public void Initialize(int levelIndex, float weight, float value, IEventsSystemService eventsSystemService, GarbageItemData itemData)
        {
            _levelIndex = levelIndex;
            _weight = weight;
            _value = value;
            _eventsSystemService = eventsSystemService;
            ItemData = itemData;
            StartFloating();
        }

        private void StartFloating()
        {
            _floatingSequence = DOTween.Sequence();

            const float horizontalOffset = 0.05f;
            var randomDuration = Random.Range(1.2f, 2.2f);

            _floatingSequence.Append(transform.DOMoveX(transform.position.x + horizontalOffset, randomDuration)
                    .SetEase(Ease.InOutSine))
                .Append(transform.DOMoveX(transform.position.x - horizontalOffset, randomDuration)
                    .SetEase(Ease.InOutSine));

            _floatingSequence.Join(transform.DOMoveY(transform.position.y + horizontalOffset, randomDuration / 2)
                    .SetEase(Ease.InOutSine))
                .Append(transform.DOMoveY(transform.position.y - horizontalOffset, randomDuration / 2)
                    .SetEase(Ease.InOutSine));

            _floatingSequence.SetLoops(-1, LoopType.Yoyo);
        }
        
        private void OnGarbageCollected()
        {
            _eventsSystemService?.Publish(ProjectConstants.Events.GARBAGE_COLLECTED, GenerateGarbageCollectedData());
        }
        
        private void OnGarbageHooked()
        {
            _floatingSequence?.Kill();
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


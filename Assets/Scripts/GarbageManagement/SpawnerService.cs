using System;
using System.Collections.Generic;
using Core;
using Data;
using EventsSystem;
using Services;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GarbageManagement
{
    public class SpawnerService : MonoService, ISpawnerService
    {
        [SerializeField]
        private LevelsConfigurationData levelsConfiguration;

        [SerializeField] 
        private Transform seaLevel;

        [SerializeField] 
        private Transform garbageHolder;

        private readonly Dictionary<int, List<Garbage>> _garbagePerLevel = new();
        private IEventsSystemService _eventSystemsService;
        private IDisposable _garbageCollectionSubscription;

        public override bool IsPersistent => false;

        protected override HashSet<Type> RequiredServices => new HashSet<Type>()
        {
            typeof(IEventsSystemService)
        };

        public void SpawnInitialGarbage()
        {
            for (int i = 0; i < levelsConfiguration.Levels.Count; i++)
            {
                GenerateAllInitialGarbageInLevel(i);
            }
        }

        public void TryRefillGarbage()
        {
            for (int levelIndex = 0; levelIndex < levelsConfiguration.Levels.Count; levelIndex++)
            {
                if (_garbagePerLevel[levelIndex].Count < levelsConfiguration.Levels[levelIndex].MinItemsForLevel)
                {
                    GenerateGarbageInLevel(levelIndex);
                }
            }
        }

        private void GenerateAllInitialGarbageInLevel(int levelIndex)
        {
            LevelData levelData = levelsConfiguration.Levels[levelIndex];

            List<Garbage> garbageInLevel = new List<Garbage>();
            _garbagePerLevel.Add(levelIndex, garbageInLevel);

            int numOfItemsToSpawn = Random.Range(levelData.MinItemsForLevel, levelData.MaxItemsForLevel + 1);

            for (int i = 0; i < numOfItemsToSpawn; i++)
            {
                GenerateGarbageInLevel(levelIndex);
            }
        }

        private void GenerateGarbageInLevel(int levelIndex)
        {
            List<Garbage> garbageInLevel = _garbagePerLevel[levelIndex];
            LevelData levelData = levelsConfiguration.Levels[levelIndex];
            float levelBaseDepth = seaLevel.position.y - levelIndex * levelsConfiguration.DepthPerLevel;
            
            float spawnPositionX = Random.Range(-levelData.HorizontalRange, levelData.HorizontalRange);
            float spawnPositionY = Random.Range(levelBaseDepth, levelBaseDepth - levelsConfiguration.DepthPerLevel);
            GarbageItemData itemToSpawn = levelData.SpawnableItems[Random.Range(0, levelData.SpawnableItems.Count)];
            var (weight, value) = itemToSpawn.CalculateRandomValue();
                
            Garbage instantiatedGarbage = Instantiate(itemToSpawn.ItemPrefab, new Vector2(spawnPositionX, spawnPositionY), quaternion.identity, garbageHolder);
            instantiatedGarbage.Initialize(levelIndex, weight, value, _eventSystemsService, itemToSpawn);
            garbageInLevel.Add(instantiatedGarbage);
        }

        protected override void Awake()
        {
            base.Awake();
            ServiceLocator.Instance.RegisterService<ISpawnerService>(this);
            Initialize();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            ServiceLocator.Instance?.UnregisterService<ISpawnerService>();
            _garbageCollectionSubscription?.Dispose();
        }

        protected override void OnServicesInitialized()
        {
            _eventSystemsService = ServiceLocator.Instance.GetService<IEventsSystemService>();
            _garbageCollectionSubscription = _eventSystemsService.Subscribe<GarbageCollectedData>(ProjectConstants.Events.GARBAGE_COLLECTED, OnGarbageCollected);
        }

        private void OnGarbageCollected(GarbageCollectedData data)
        {
            if (_garbagePerLevel[data.LevelIndex]?.Remove(data.Garbage) is true)
            {
                Destroy(data.Garbage.gameObject);
            }
            else
            {
                MgLogger.LogError($"Garbage with level index {data.LevelIndex} not found in the list.");
            }
        }
    }
}
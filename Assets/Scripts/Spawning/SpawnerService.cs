using Data;
using Services;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawning
{
    public class SpawnerService : MonoService, ISpawnerService
    {
        [SerializeField]
        private LevelsConfigurationData levelsConfiguration;

        [SerializeField] 
        private Transform seaLevel;

        [SerializeField] 
        private Transform garbageHolder;
        
        public override bool IsPersistent => false;

        public void SpawnInitialGarbage()
        {
            for (int i = 0; i < levelsConfiguration.Levels.Count; i++)
            {
                GenerateGarbageInLevel(i);
            }
        }

        public void RefillGarbage()
        {
        }

        private void GenerateGarbageInLevel(int levelIndex)
        {
            float levelBaseDepth = seaLevel.position.y - levelIndex * levelsConfiguration.DepthPerLevel;
            LevelData levelData = levelsConfiguration.Levels[levelIndex];

            int numOfItemsToSpawn = Random.Range(levelData.MinItemsForLevel, levelData.MaxItemsForLevel + 1);

            for (int i = 0; i < numOfItemsToSpawn; i++)
            {
                float spawnPositionX = Random.Range(-levelData.HorizontalRange, levelData.HorizontalRange);
                float spawnPositionY = Random.Range(levelBaseDepth, levelBaseDepth - levelsConfiguration.DepthPerLevel);
                GarbageItemData itemToSpawn = levelData.SpawnableItems[Random.Range(0, levelData.SpawnableItems.Count)];
                var (weight, value) = itemToSpawn.CalculateRandomValue();
                
                Garbage instantiatedGarbage = Instantiate(itemToSpawn.ItemPrefab, new Vector2(spawnPositionX, spawnPositionY), quaternion.identity, garbageHolder);
                instantiatedGarbage.Initialize(weight, value);
            }
        }
        
        protected void Awake()
        {
            ServiceLocator.Instance.RegisterService<ISpawnerService>(this);
            Initialize();
        }
        
        protected void OnDestroy()
        {
            Shutdown();
            ServiceLocator.Instance?.UnregisterService<ISpawnerService>();
        }
    }
}
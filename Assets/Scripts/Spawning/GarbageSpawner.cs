using Data;
using Services;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawning
{
    public class GarbageSpawner : MonoService, IGarbageSpawner
    {
        [SerializeField]
        private LevelsConfigurationData levelsConfiguration;

        [SerializeField] 
        private Transform seaLevel;

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
                float spawnPositionY = Random.Range(levelBaseDepth, levelBaseDepth + levelsConfiguration.DepthPerLevel);
                var prefabToSpawn = levelData.SpawnableItems[Random.Range(0, levelData.SpawnableItems.Count)].ItemPrefab;

                Instantiate(prefabToSpawn, new Vector2(spawnPositionX, spawnPositionY), quaternion.identity);
            }
        }
    }
}
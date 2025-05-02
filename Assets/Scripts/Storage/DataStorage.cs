using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;
using Core;

namespace Storage
{
    public class DataStorageService : IDataStorageService
    {
        private const string PLAYER_PREFS_KEY = nameof(GameData);
        private readonly HashSet<string> _gameDataKeys;
        private GameData _gameData;
        
        public GameData GetGameData()
        {
            return _gameData ??= LoadData<GameData>(PLAYER_PREFS_KEY);
        }
        
        public DataStorageService()
        {
            _gameDataKeys = typeof(GameData).GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Select(field => field.Name)
                .ToHashSet();
        }
        
        public void Initialize()
        {
            LoadGameData();
        }

        public void Shutdown()
        {
            PlayerPrefs.Save();
        }
        
        public void SaveData<T>(string key, T data) where T : class
        {
            try
            {
                var jsonData = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(key, jsonData);
            }
            catch (Exception e)
            {
                MgLogger.LogError($"Failed to save data: {e.Message}");
            }
        }

        public T LoadData<T>(string key) where T : class, new()
        {
            try
            {
                var jsonData = PlayerPrefs.GetString(key, string.Empty);
                
                return string.IsNullOrEmpty(jsonData) ? new T() : JsonUtility.FromJson<T>(jsonData);
            }
            catch (Exception e)
            {
                MgLogger.LogError($"Failed to load data: {e.Message}");
                return new T();
            }
        }

        private void LoadGameData()
        {
            try
            {
                var jsonData = PlayerPrefs.GetString(PLAYER_PREFS_KEY, string.Empty);
                _gameData = string.IsNullOrEmpty(jsonData) ? new GameData() : JsonUtility.FromJson<GameData>(jsonData);
            }
            catch (Exception e)
            {
                MgLogger.LogError($"Failed to load game data: {e.Message}");
                _gameData = new GameData();
            }
        }

        public bool ModifyGameDataSync(Action<GameData> modifier)
        {
            return ModifyGameDataSync(gameData =>
            {
                modifier.Invoke(gameData);
                return true;
            });
        }

        public bool ModifyGameDataSync(Func<GameData, bool> modifier)
        {
            var copyOfGameData = _gameData.Copy();
            var didChange = modifier.Invoke(copyOfGameData);

            if (didChange)
            {
                _gameData = copyOfGameData;
                
                try
                {
                    var jsonData = JsonUtility.ToJson(_gameData);
                    PlayerPrefs.SetString(PLAYER_PREFS_KEY, jsonData);
                }
                catch (Exception e)
                {
                    MgLogger.LogError($"Failed to save game data: {e.Message}");
                }
            }

            return didChange;
        }
    }
} 
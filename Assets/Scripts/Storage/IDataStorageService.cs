using Services;
using System;

namespace Storage
{
    public interface IDataStorageService : IService
    {
        public GameData GetGameData();
        public void SaveData<T>(string key, T data) where T : class;
        public T LoadData<T>(string key) where T : class, new();
        public bool ModifyGameDataSync(Action<GameData> modifier);
        public bool ModifyGameDataSync(Func<GameData, bool> modifier);
    }
}
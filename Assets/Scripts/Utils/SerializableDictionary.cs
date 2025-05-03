using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new List<TKey>();
        [SerializeField] private List<TValue> values = new List<TValue>();

        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public Dictionary<TKey, TValue> Dictionary => dictionary;

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var kvp in dictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();
            for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
            {
                dictionary[keys[i]] = values[i];
            }
        }

        public void Add(TKey key, TValue value) => dictionary[key] = value;
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
        public void Clear() => dictionary.Clear();
        public int Count => dictionary.Count;

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }
    }


}
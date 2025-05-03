using System;
using UnityEngine.Serialization;

namespace Storage
{
    [Serializable]
    public class GameData
    {
        [FormerlySerializedAs("DepthReached")] public int depthReached;
        [FormerlySerializedAs("Currency")] public int currency = 0;

        public GameData Copy()
        {
            return new GameData
            {
                depthReached = depthReached,
                currency = currency,
            };
        }
    }
}
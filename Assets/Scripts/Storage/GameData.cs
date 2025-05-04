using System;
using Models;
using UnityEngine.Serialization;

namespace Storage
{
    [Serializable]
    public class GameData
    {
        public int depthReached; 
        public int currency = 0;
        public UpgradesModel upgradesModel;

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
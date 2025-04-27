using System;

namespace Storage
{
    [Serializable]
    public class GameData
    {
        public int DepthReached;

        public GameData Copy()
        {
            return new GameData
            {
                DepthReached = DepthReached
            };
        }
    }
}
namespace GarbageManagement
{
    public readonly struct GarbageCollectedData
    {
        public GarbageCollectedData(int levelIndex, float weight, float value)
        {
            LevelIndex = levelIndex;
            Weight = weight;
            Value = value;
        }
        
        public int LevelIndex { get; }
        public float Weight { get; }
        public float Value { get; }
    }
}
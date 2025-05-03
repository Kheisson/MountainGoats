namespace GarbageManagement
{
    public readonly struct GarbageCollectedData
    {
        public GarbageCollectedData(int levelIndex, float weight, float value, Garbage garbage)
        {
            LevelIndex = levelIndex;
            Weight = weight;
            Value = value;
            Garbage = garbage;
        }
        
        public int LevelIndex { get; }
        public float Weight { get; }
        public float Value { get; }
        public Garbage Garbage { get; }
    }
}
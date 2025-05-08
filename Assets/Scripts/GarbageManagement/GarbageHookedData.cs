namespace GarbageManagement
{
    public struct GarbageHookedData
    {
        public GarbageHookedData(Garbage garbage)
        {
            Garbage = garbage;
        }

        public Garbage Garbage { get; }
    }
}
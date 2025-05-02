using Services;

namespace Spawning
{
    public interface IGarbageSpawner : IService
    {
        void SpawnInitialGarbage();

        void RefillGarbage();
    }
}
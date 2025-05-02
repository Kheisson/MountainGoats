using Services;

namespace Spawning
{
    public interface ISpawnerService : IService
    {
        void SpawnInitialGarbage();

        void RefillGarbage();
    }
}
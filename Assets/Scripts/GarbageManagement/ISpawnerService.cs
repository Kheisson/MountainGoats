using Services;

namespace GarbageManagement
{
    public interface ISpawnerService : IService
    {
        void SpawnInitialGarbage();

        void TryRefillGarbage();
    }
}
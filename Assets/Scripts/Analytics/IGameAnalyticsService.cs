using Services;

namespace Analytics
{
    public interface IGameAnalyticsService : IService
    {
        void SendEvent(GameEvent gameEvent);
    }
}
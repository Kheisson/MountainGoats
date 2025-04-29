using System;
using Unity.Services.Analytics;

namespace Analytics
{
    public class UnityGameAnalyticsService : IGameAnalyticsService
    {
        public void Initialize()
        {
            AnalyticsService.Instance.StartDataCollection();
        }

        public void Shutdown()
        {
            AnalyticsService.Instance.Flush();
        }

        public void SendEvent(GameEvent gameEvent)
        {
            if (gameEvent == null)
            {
                throw new ArgumentNullException(nameof(gameEvent), "GameEvent cannot be null");
            }

            AnalyticsService.Instance.RecordEvent(gameEvent);
        }
    }
}
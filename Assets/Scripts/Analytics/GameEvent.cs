using Core;
using Services;
using Storage;
using Unity.Services.Analytics;

namespace Analytics
{
    public abstract class GameEvent : Event
    {
        protected IDataStorageService DataStorageService { get; private set; }
        private IGameAnalyticsService GameAnalyticsService { get; }
        
        protected GameEvent(string name) : base(name)
        {
            GameAnalyticsService = ServiceLocator.Instance.GetService<IGameAnalyticsService>();
            DataStorageService = ServiceLocator.Instance.GetService<IDataStorageService>();
        }

        protected abstract bool ValidateEvent();

        private void SendEvent()
        {
            MgLogger.Log("Sending event", this);
            GameAnalyticsService.SendEvent(this);
        }

        public void TrySendEvent()
        {
            if (ValidateEvent())
            {
                SendEvent();
            }
        }
    }
}
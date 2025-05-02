using System;
using Services;

namespace EventsSystem
{
    public interface IEventsSystemService : IService
    {
        IDisposable Subscribe<TData>(string eventID, Action<TData> onInvoke);

        IDisposable Subscribe(string eventID, Action onInvoke);

        void Publish<TData>(string eventID, TData messageData);
        void Publish(string eventID);
    }
}
using System;

namespace EventsSystem
{
    public interface IEventsSystemService
    {
        IDisposable Subscribe<TData>(string eventID, Action<TData> onInvoke);

        void Publish<TData>(TData messageData);
    }
}
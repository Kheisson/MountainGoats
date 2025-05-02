using System;
using System.Collections.Generic;

namespace EventsSystem
{
    public class EventsSystemService : IEventsSystemService
    {
        private Dictionary<string, object> _observers;
            
        public IDisposable Subscribe<TData>(string eventID, Action<TData> onInvoke)
        {
            _observers.Add(eventID, onInvoke);
            return null;
        }

        public void Publish<TData>(TData messageData)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using Services;
using UniRx;

namespace EventsSystem
{
    public class EventsSystemService : IEventsSystemService
    {
        private readonly Dictionary<string, List<object>> _observersById = new();
            
        public IDisposable Subscribe<TData>(string eventID, Action<TData> onInvoke)
        {
            return SubscribeInternal(eventID, onInvoke);
        }
        
        public IDisposable Subscribe(string eventID, Action onInvoke)
        {
            return SubscribeInternal(eventID, onInvoke);
        }

        private IDisposable SubscribeInternal(string eventID, object onInvoke)
        {
            if (_observersById.ContainsKey(eventID))
            {
                _observersById[eventID].Add(onInvoke);
            }
            else
            {
                _observersById.Add(eventID, new List<object>(){onInvoke});
            }
            
            return Disposable.Create(() =>
            {
                if (!_observersById.TryGetValue(eventID, out List<object> observers))
                {
                    return; 
                }

                observers.Remove(onInvoke);
                
                if (observers.Count == 0)
                {
                    _observersById.Remove(eventID);
                }
            });
        }

        public void Publish<TData>(string eventID, TData messageData)
        {
            if (!_observersById.TryGetValue(eventID, out List<object> observers))
            {
                return;
            }

            foreach (var observer in observers)
            {
                if (observer is Action<TData> action)
                {
                    action.Invoke(messageData);
                }
            }
        }

        public void Publish(string eventID)
        {
            if (!_observersById.TryGetValue(eventID, out List<object> observers))
            {
                return;
            }

            foreach (var observer in observers)
            {
                if (observer is Action action)
                {
                    action?.Invoke();
                }
            }
        }

        public void Initialize()
        {
        }

        public void Shutdown()
        {
        }
    }
}
using System;
using System.Collections.Generic;
using Core;
using EventsSystem;
using Services;
using GarbageManagement;

namespace GameManagement
{
    public class GameManager : BaseMonoBehaviour
    {
        private ISpawnerService _spawnerService;
        private IEventsSystemService _eventsSystemService;
        private IDisposable _playStartedSubscription;
        private IDisposable _playEndedSubscription;

        protected override HashSet<Type> RequiredServices => new HashSet<Type>()
        {
            typeof(ISpawnerService),
            typeof(IEventsSystemService)
        };

        protected override void OnServicesInitialized()
        {
            _spawnerService = ServiceLocator.Instance.GetService<ISpawnerService>();
            _eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
            
            _playStartedSubscription = _eventsSystemService.Subscribe(ProjectConstants.Events.PLAY_STARTED, OnPlayStarted);
            _playEndedSubscription = _eventsSystemService.Subscribe(ProjectConstants.Events.PLAY_ENDED, OnPlayEnded);
            
            _spawnerService.SpawnInitialGarbage();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _playStartedSubscription?.Dispose();
            _playEndedSubscription?.Dispose();
        }

        private void OnPlayStarted()
        {
        }
        
        private void OnPlayEnded()
        {
            _spawnerService.TryRefillGarbage();
        }
    }
}
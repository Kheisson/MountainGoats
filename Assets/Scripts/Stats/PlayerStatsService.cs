using System;
using System.Collections.Generic;
using Core;
using EventsSystem;
using Models;
using Services;
using Storage;
using UnityEngine;
using Upgrades;

namespace Stats
{
    public class PlayerStatsService : MonoService, IDisposable, IPlayerStatsService
    {
        [SerializeField] private UpgradePathsTable availableUpgrades;
        [SerializeField] private PlayerStatsBase basePlayerStats;

        private IDisposable _upgradesUpdatedSubscription;
        private IPlayerStats _finalPlayerStats;

        protected override HashSet<Type> RequiredServices => new HashSet<Type>()
        {
            typeof(IDataStorageService),
            typeof(IEventsSystemService)
        };
        
        protected override void OnServicesInitialized()
        {
            var eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
            var dataStorageService = ServiceLocator.Instance.GetService<IDataStorageService>();
            
            _upgradesUpdatedSubscription = eventsSystemService.Subscribe<UpgradesModel>(ProjectConstants.Events.UPGRADE_PURCHASED, model =>
            {
                _finalPlayerStats = ApplyUpgrades(model);
            });
            
            var upgradesModel = dataStorageService.GetGameData().upgradesModel;
            _finalPlayerStats = ApplyUpgrades(upgradesModel);
        }

        private IPlayerStats ApplyUpgrades(UpgradesModel upgradesModel)
        {
            IPlayerStats playerStats = basePlayerStats;
            
            foreach (var upgradeType in availableUpgrades.UpgradePaths.Keys)
            {
                if (upgradesModel.TryGetUpgradesByType(upgradeType, out var maxUpgradeIndex))
                {
                    playerStats = availableUpgrades.ApplyUpgrade(playerStats, upgradeType, maxUpgradeIndex);
                }
            }

            return playerStats;
        }

        public float RopeLength => _finalPlayerStats.RopeLength;
        public float HorizontalSpeed => _finalPlayerStats.HorizontalSpeed;
        public float LightIntensity => _finalPlayerStats.LightIntensity;
        public float EnergyCapacity => _finalPlayerStats.EnergyCapacity;

        public void Dispose()
        {
            _upgradesUpdatedSubscription?.Dispose();
        }

        public override bool IsPersistent { get; } = true;

        protected override void Awake()
        {
            base.Awake();
            ServiceLocator.Instance.RegisterService<IPlayerStatsService>(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ServiceLocator.Instance.UnregisterService<IPlayerStatsService>();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            Dispose();
        }
    }
}
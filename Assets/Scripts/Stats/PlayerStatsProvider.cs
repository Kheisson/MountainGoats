using System;
using Core;
using EventsSystem;
using Models;
using Storage;
using Upgrades;

namespace Stats
{
    public class PlayerStatsProvider : IPlayerStats, IDisposable
    {
        private readonly IPlayerStats _basePlayerStats;
        private readonly UpgradePathsTable _availableUpgrades;
        private readonly IDisposable _upgradesUpdatedSubscription;
        
        private IPlayerStats _playerStats;
        
        public PlayerStatsProvider(IPlayerStats basePlayerStats, IDataStorageService dataStorageService, IEventsSystemService eventsSystemService,
            UpgradePathsTable availableUpgrades)
        {
            _basePlayerStats = basePlayerStats;
            _availableUpgrades = availableUpgrades;
            _upgradesUpdatedSubscription = eventsSystemService.Subscribe<UpgradesModel>(ProjectConstants.Events.UPGRADE_PURCHASED, model =>
            {
                _playerStats = ApplyUpgrades(model);
            });
            
            var upgradesModel = dataStorageService.GetGameData().upgradesModel;
            _playerStats = ApplyUpgrades(upgradesModel);
        }

        private IPlayerStats ApplyUpgrades(UpgradesModel upgradesModel)
        {
            var playerStats = _basePlayerStats;
            
            foreach (var upgradeType in _availableUpgrades.UpgradePaths.Keys)
            {
                if (upgradesModel.TryGetUpgradesByType(upgradeType, out var maxUpgradeIndex))
                {
                    playerStats = _availableUpgrades.ApplyUpgrade(playerStats, upgradeType, maxUpgradeIndex);
                }
            }

            return playerStats;
        }

        public float RopeLength => _playerStats.RopeLength;
        public float HorizontalSpeed => _playerStats.HorizontalSpeed;
        public float LightIntensity => _playerStats.LightIntensity;
        public float EnergyCapacity => _playerStats.EnergyCapacity;

        public void Dispose()
        {
            _upgradesUpdatedSubscription?.Dispose();
        }
    }
}
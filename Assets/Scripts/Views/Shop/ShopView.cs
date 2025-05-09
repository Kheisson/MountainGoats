using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Events;
using EventsSystem;
using Services;
using Storage;
using UniRx;
using UnityEngine;
using Upgrades;

namespace Views.Shop
{
    public class ShopView : BaseMonoBehaviour
    {
        [SerializeField] private Transform upgradesHolder;
        [SerializeField] private UpgradePathView upgradePathPrefab;
        [SerializeField] private UpgradePathsTable upgradePathsTable;
        [SerializeField] private ShopDescriptionView descriptionView;

        private readonly Dictionary<EUpgradeType, UpgradePathView> _upgradePathViews = new ();
        private Dictionary<EUpgradeType, int> _purchasedUpgrades;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        protected override HashSet<Type> RequiredServices => new HashSet<Type>()
        {
            typeof(IDataStorageService),
            typeof(IEventsSystemService)
        };
        
        protected override void OnServicesInitialized()
        {
            var upgradesModel = ServiceLocator.Instance.GetService<IDataStorageService>().GetGameData().purchasedUpgrades;
            var eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
            
            eventsSystemService.Subscribe<Dictionary<EUpgradeType, int>>(
                ProjectConstants.Events.UPGRADE_PURCHASED, RefreshView)
                .AddTo(_disposables);

            eventsSystemService.Subscribe<UpgradeHoverEvent>(ProjectConstants.Events.ICON_HOVER, OnUpgradeHover)
                .AddTo(_disposables);
            
            eventsSystemService.Subscribe(ProjectConstants.Events.ICON_HOVER_ENDED, OnUpgradeHoverEnded)
                    .AddTo(_disposables);
            
            RefreshView(upgradesModel);
            descriptionView.Hide();
        }

        private void OnUpgradeHover(UpgradeHoverEvent hoverEvent)
        {
            descriptionView.Display(hoverEvent.UpgradeData, hoverEvent.IsPurchased);
        }
        
        private void OnUpgradeHoverEnded()
        {
            descriptionView.Hide();
        }

        private void RefreshView(Dictionary<EUpgradeType, int> upgradesModel)
        {
            _purchasedUpgrades = upgradesModel;
            descriptionView.Hide();
            
            foreach (var upgradeType in upgradePathsTable.UpgradePaths.Keys)
            {
                SetupUpgradeView(upgradeType);
            }
        }

        private void SetupUpgradeView(EUpgradeType upgradeType)
        {
            var upgradePath = upgradePathsTable.UpgradePaths[upgradeType];
            var availableUpgrades = upgradePath.AvailableUpgrades;

            int maxIndex;
            
            if (_purchasedUpgrades.TryGetValue(upgradeType, out var purchasedIndex))
            {
                maxIndex = purchasedIndex;
            }
            else
            {
                maxIndex = -1;
            }

            if (!_upgradePathViews.ContainsKey(upgradeType))
            {
                var instantiatedUpgradePath = Instantiate(upgradePathPrefab, upgradesHolder);
                _upgradePathViews[upgradeType] = instantiatedUpgradePath;
            }
            
            _upgradePathViews[upgradeType].UpdateView(availableUpgrades, maxIndex, upgradePath, upgradeType);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _disposables?.Dispose();
        }
    }
}
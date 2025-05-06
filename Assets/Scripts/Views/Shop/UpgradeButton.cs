using System;
using System.Collections.Generic;
using Core;
using EventsSystem;
using Services;
using UnityEngine;
using UnityEngine.UIElements;
using Upgrades;
using Button = UnityEngine.UI.Button;

namespace Views.Shop
{
    public class UpgradeButton : BaseMonoBehaviour
    {
        private IEventsSystemService _eventsSystemService;
        [SerializeField] private Button button;
        
        private int _buttonIndex;
        private UpgradePath _upgradePath;
        private EUpgradeType _upgradeType;

        public UpgradeButton(IEventsSystemService eventsSystemService)
        {
            _eventsSystemService = eventsSystemService;
        }

        protected override HashSet<Type> RequiredServices => new HashSet<Type>()
        {
            typeof(IEventsSystemService)
        };

        protected override void OnServicesInitialized()
        {
            _eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            _eventsSystemService.Publish(ProjectConstants.Events.PURCHASE_BUTTON_CLICKED, 
                new PurchaseButtonClickedEvent(_upgradePath, _buttonIndex, _upgradeType));
        }
        
        public void SetupButton(int buttonIndex, UpgradePath upgradePath, EUpgradeType upgradeType)
        {
            _buttonIndex = buttonIndex;
            _upgradePath = upgradePath;
            _upgradeType = upgradeType;
        }
    }
}
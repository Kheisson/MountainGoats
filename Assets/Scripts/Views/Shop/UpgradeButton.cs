using System;
using System.Collections.Generic;
using Core;
using EventsSystem;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Upgrades;
using Button = UnityEngine.UI.Button;

namespace Views.Shop
{
    public class UpgradeButton : BaseMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI costText;
        
        private int _buttonIndex;
        private UpgradePath _upgradePath;
        private EUpgradeType _upgradeType;
        private IEventsSystemService _eventsSystemService;

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
        
        public void SetupButton(int upgradeIndex, UpgradePath upgradePath, EUpgradeType upgradeType)
        {
            _buttonIndex = upgradeIndex;
            _upgradePath = upgradePath;
            _upgradeType = upgradeType;

            costText.text = $"UPGRADE'\n'{upgradePath.AvailableUpgrades[_buttonIndex].Cost}$";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _eventsSystemService?.Publish(ProjectConstants.Events.ICON_HOVER, _upgradePath.AvailableUpgrades[_buttonIndex]);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _eventsSystemService?.Publish(ProjectConstants.Events.ICON_HOVER_ENDED);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _eventsSystemService?.Publish(ProjectConstants.Events.ICON_HOVER_ENDED);
        }
    }
}
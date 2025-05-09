using System;
using System.Collections.Generic;
using Core;
using EventsSystem;
using Services;
using UnityEngine.EventSystems;

namespace UI
{
    public class ClicksBlocker : BaseMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private IEventsSystemService _eventsSystemService;
        
        protected override HashSet<Type> RequiredServices { get; } = new HashSet<Type>
        {
            typeof(IEventsSystemService),
        };

        protected override void OnServicesInitialized()
        {
            _eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _eventsSystemService.Publish(ProjectConstants.Events.CLICKS_BLOCKED, new ClicksBlockedData(true));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _eventsSystemService.Publish(ProjectConstants.Events.CLICKS_BLOCKED, new ClicksBlockedData(false));
        }
    }
}
using Controllers;
using Core;
using EventsSystem;
using Services;
using Storage;
using UnityEngine;
using Views;

namespace Managers
{
    public class NotebookManager : MonoService, INotebookManager
    {
        [Header("UI References")]
        [SerializeField] private GameObject notebookGameObject;
        [SerializeField] private NotebookItemView itemViewPrefab;
        [SerializeField] private Transform contentParent;
        [SerializeField] private NotebookPageView pageView;
        [SerializeField] private UnityEngine.UI.Button prevButton;
        [SerializeField] private UnityEngine.UI.Button nextButton;
        
        private NotebookController _notebookController;
        private IDataStorageService _dataStorage;
        private IEventsSystemService _eventsSystemService;
        
        public override bool IsPersistent => false;
        public bool IsNotebookOpen { get; private set; } = false;


        protected override void OnServicesInitialized()
        {
            _dataStorage = ServiceLocator.Instance.GetService<IDataStorageService>();
            _eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
            
            InitializeNotebook();
            ServiceLocator.Instance.RegisterService<INotebookManager>(this);
        }
        
        private void InitializeNotebook()
        {
            _notebookController = new NotebookController(
                _dataStorage,
                _eventsSystemService,
                itemViewPrefab,
                contentParent,
                pageView,
                prevButton,
                nextButton
            );
            
            HideNotebook();
        }
        
        public void ShowNotebook()
        {
            if (notebookGameObject == null)
            {
                MgLogger.LogError("Notebook is not assigned.");
                return;
            }
            
            notebookGameObject.SetActive(true);
            IsNotebookOpen = true;
        }
        
        public void HideNotebook()
        {
            if (notebookGameObject == null)
            {
                MgLogger.LogError("Notebook is not assigned.");
                return;
            }
    
            notebookGameObject.SetActive(false);
            IsNotebookOpen = false;
        }
        
        public void UpdateNotebook()
        {
            if (_notebookController == null)
            {
                MgLogger.LogError("NotebookController is not initialized.");
                return;
            }
            
            _notebookController.UpdateNotebook();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ServiceLocator.Instance?.UnregisterService<INotebookManager>();
        }
    }
} 
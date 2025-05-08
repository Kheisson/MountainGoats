using Controllers;
using Core;
using EventsSystem;
using Services;
using Storage;
using UnityEngine;
using Views;
using System.Collections.Generic;

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
        
        [Header("Tab Settings")]
        [SerializeField] private List<GameObject> tabPages;
        [SerializeField] private List<UnityEngine.UI.Button> tabButtons;
        
        private IndexController _indexController;
        private NotebookTabController _tabController;
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
            _indexController = new IndexController(
                _dataStorage,
                _eventsSystemService,
                itemViewPrefab,
                contentParent,
                pageView,
                prevButton,
                nextButton
            );
            
            _tabController = new NotebookTabController(
                _dataStorage,
                tabPages,
                tabButtons
            );
            
            HideNotebook();
            UpdateNotebook();
        }
        
        public void ShowNotebook()
        {
            if (notebookGameObject == null)
            {
                MgLogger.LogError("Notebook is not assigned.");
                return;
            }
            
            Time.timeScale = 0;
            _eventsSystemService.Publish(ProjectConstants.Events.GAME_PAUSED);
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
    
            Time.timeScale = 1;
            _eventsSystemService.Publish(ProjectConstants.Events.GAME_RESUMED);
            notebookGameObject.SetActive(false);
            IsNotebookOpen = false;
        }
        
        public void UpdateNotebook()
        {
            if (_indexController == null)
            {
                MgLogger.LogError("NotebookController is not initialized.");
                return;
            }
            
            _indexController.UpdateNotebook();
        }
        
        public override void Shutdown()
        {
            base.Shutdown();
            ServiceLocator.Instance?.UnregisterService<INotebookManager>();
        }
    }
} 
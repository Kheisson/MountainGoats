using System.Collections.Generic;
using Core;
using Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class NotebookTabController
    {
        private readonly IDataStorageService _dataStorage;
        private readonly List<GameObject> _tabPages;
        private readonly List<Button> _tabButtons;
        private int _currentTabIndex = -1;
        
        public NotebookTabController(
            IDataStorageService dataStorage,
            List<GameObject> tabPages,
            List<Button> tabButtons)
        {
            _dataStorage = dataStorage;
            _tabPages = tabPages;
            _tabButtons = tabButtons;
            
            Initialize();
        }
        
        private void Initialize()
        {
            if (_tabPages.Count != _tabButtons.Count)
            {
                MgLogger.LogError("Number of tab pages must match number of tab buttons");
                return;
            }
            
            SetupTabButtons();
            LoadLastSelectedTab();
        }
        
        private void SetupTabButtons()
        {
            for (int i = 0; i < _tabButtons.Count; i++)
            {
                var index = i; 
                _tabButtons[i].onClick.AddListener(() => SwitchTab(index));
            }
        }
        
        private void LoadLastSelectedTab()
        {
            var lastSelectedTab = _dataStorage.GetGameData().lastSelectedTabIndex;
            var tabIndex = lastSelectedTab >= 0 && lastSelectedTab < _tabPages.Count 
                ? lastSelectedTab 
                : 0;
            
            SwitchTab(tabIndex);
        }
        
        private void SwitchTab(int index)
        {
            if (index < 0 || index >= _tabPages.Count) return;
            
            if (_currentTabIndex >= 0 && _currentTabIndex < _tabPages.Count)
            {
                _tabPages[_currentTabIndex].SetActive(false);
                _tabButtons[_currentTabIndex].interactable = true;
            }
            
            _currentTabIndex = index;
            _tabPages[_currentTabIndex].SetActive(true);
            _tabButtons[_currentTabIndex].interactable = false;
            
            _dataStorage.ModifyGameDataSync(gameData =>
            {
                gameData.lastSelectedTabIndex = _currentTabIndex;
                return true;
            });
        }
    }
} 
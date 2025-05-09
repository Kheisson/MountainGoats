using System.Collections.Generic;
using Core;
using Storage;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Controllers
{
    public class NotebookTabController
    {
        private readonly IDataStorageService _dataStorage;
        private readonly List<GameObject> _tabPages;
        private readonly List<Button> _tabButtons;
        private readonly List<RectTransform> _tabButtonTransforms;
        private readonly List<Vector2> _originalPositions;
        private int _currentTabIndex = -1;
        
        private const float BUTTON_LIFT_AMOUNT = 10f;
        private const float ANIMATION_DURATION = 0.2f;
        
        public NotebookTabController(
            IDataStorageService dataStorage,
            List<GameObject> tabPages,
            List<Button> tabButtons)
        {
            _dataStorage = dataStorage;
            _tabPages = tabPages;
            _tabButtons = tabButtons;
            _tabButtonTransforms = new List<RectTransform>();
            _originalPositions = new List<Vector2>();
            
            Initialize();
        }
        
        private void Initialize()
        {
            if (_tabPages.Count != _tabButtons.Count)
            {
                MgLogger.LogError("Number of tab pages must match number of tab buttons");
                return;
            }
            
            foreach (var button in _tabButtons)
            {
                var rectTransform = button.GetComponent<RectTransform>();
                _tabButtonTransforms.Add(rectTransform);
                _originalPositions.Add(rectTransform.anchoredPosition);
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
            
            _currentTabIndex = tabIndex;
            _tabPages[_currentTabIndex].SetActive(true);
            _tabButtons[_currentTabIndex].interactable = false;
            
            _tabButtonTransforms[_currentTabIndex]
                .DOAnchorPosY(_originalPositions[_currentTabIndex].y + BUTTON_LIFT_AMOUNT, ANIMATION_DURATION)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }
        
        private void SwitchTab(int index)
        {
            if (index < 0 || index >= _tabPages.Count) return;
            
            if (_currentTabIndex >= 0 && _currentTabIndex < _tabPages.Count)
            {
                _tabPages[_currentTabIndex].SetActive(false);
                _tabButtons[_currentTabIndex].interactable = true;
                
                _tabButtonTransforms[_currentTabIndex]
                    .DOAnchorPosY(_originalPositions[_currentTabIndex].y, ANIMATION_DURATION)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true);
            }
            
            _currentTabIndex = index;
            _tabPages[_currentTabIndex].SetActive(true);
            _tabButtons[_currentTabIndex].interactable = false;
            
            _tabButtonTransforms[_currentTabIndex]
                .DOAnchorPosY(_originalPositions[_currentTabIndex].y + BUTTON_LIFT_AMOUNT, ANIMATION_DURATION)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
            
            _dataStorage.ModifyGameDataSync(gameData =>
            {
                gameData.lastSelectedTabIndex = _currentTabIndex;
                return true;
            });
        }
    }
} 
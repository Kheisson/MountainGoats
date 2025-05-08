using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using EventsSystem;
using GarbageManagement;
using Storage;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace Controllers
{
    public class NotebookController
    {
        private readonly IDataStorageService _dataStorage;
        private readonly IEventsSystemService _eventsSystemService;
        private readonly NotebookItemView _itemViewPrefab;
        private readonly Transform _contentParent;
        private readonly NotebookPageView _pageView;
        private readonly Button _prevButton;
        private readonly Button _nextButton;
        private readonly List<NotebookItemView> _itemViews = new();
        
        private int _currentSelectedIndex = -1;
        private List<GarbageItemData> _allItems;
        
        public NotebookController(
            IDataStorageService dataStorage,
            IEventsSystemService eventsSystemService,
            NotebookItemView itemViewPrefab,
            Transform contentParent,
            NotebookPageView pageView,
            Button prevButton,
            Button nextButton)
        {
            _dataStorage = dataStorage;
            _eventsSystemService = eventsSystemService;
            _itemViewPrefab = itemViewPrefab;
            _contentParent = contentParent;
            _pageView = pageView;
            _prevButton = prevButton;
            _nextButton = nextButton;
            
            Initialize();
        }
        
        private void Initialize()
        {
            LoadAllItems();
            CreateItemViews();
            SetupNavigationButtons();
            
            _eventsSystemService.Subscribe<GarbageCollectedData>(ProjectConstants.Events.GARBAGE_COLLECTED, OnGarbageCollected);
        }
        
        private void LoadAllItems()
        {
            _allItems = Resources.LoadAll<GarbageItemData>("GarbageItems").ToList();
        }
        
        private void CreateItemViews()
        {
            foreach (var itemData in _allItems)
            {
                var itemView = Object.Instantiate(_itemViewPrefab, _contentParent);
                var isUnlocked = _dataStorage.GetGameData().unlockedItems.Contains(itemData.name);
                
                itemView.Initialize(itemData, isUnlocked, OnItemSelected);
                _itemViews.Add(itemView);
            }
            
            var lastSelectedId = _dataStorage.GetGameData().lastSelectedItemId;
            var indexToSelect = string.IsNullOrEmpty(lastSelectedId) ? 0 
                : _allItems.FindIndex(item => item.name == lastSelectedId);
            
            if (indexToSelect < 0) indexToSelect = 0;
            
            SelectItem(indexToSelect);
        }
        
        private void SetupNavigationButtons()
        {
            _prevButton?.onClick.AddListener(() => NavigateItems(-1));
            _nextButton?.onClick.AddListener(() => NavigateItems(1));
        }
        
        private void OnItemSelected(NotebookItemView selectedView)
        {
            var index = _itemViews.IndexOf(selectedView);
            
            if (index >= 0)
            {
                SelectItem(index);
            }
        }
        
        private void SelectItem(int index)
        {
            if (index < 0 || index >= _itemViews.Count) return;
            
            if (_currentSelectedIndex >= 0 && _currentSelectedIndex < _itemViews.Count)
            {
                _itemViews[_currentSelectedIndex].SetSelected(false);
            }
            
            _currentSelectedIndex = index;
            var selectedView = _itemViews[_currentSelectedIndex];
            selectedView.SetSelected(true);
            _pageView.UpdateContent(selectedView.ItemData, selectedView.IsUnlocked);
            
            _dataStorage.ModifyGameDataSync(gameData =>
            {
                gameData.lastSelectedItemId = selectedView.ItemData.name;
                return true;
            });
        }
        
        private void NavigateItems(int direction)
        {
            if (_itemViews.Count == 0) return;
            
            var newIndex = (_currentSelectedIndex + direction + _itemViews.Count) % _itemViews.Count;
            SelectItem(newIndex);
        }
        
        private void OnGarbageCollected(GarbageCollectedData data)
        {
            var itemName = data.Garbage.ItemData.ItemName;
            
            _dataStorage.ModifyGameDataSync(gameData =>
            {
                if (gameData.unlockedItems.Contains(itemName)) return false;
                
                gameData.unlockedItems.Add(data.Garbage.ItemData.ItemName);
                
                return true;
            });
            
            UpdateNotebook();
        }

        public void UpdateNotebook()
        {
            foreach (var itemView in _itemViews)
            {
                var isUnlocked = _dataStorage.GetGameData().unlockedItems.Contains(itemView.ItemData.ItemName);
                itemView.Initialize(itemView.ItemData, isUnlocked, OnItemSelected);
            }

            if (_currentSelectedIndex < 0 || _currentSelectedIndex >= _itemViews.Count) return;
            {
                var selectedView = _itemViews[_currentSelectedIndex];
                var isUnlocked = _dataStorage.GetGameData().unlockedItems.Contains(selectedView.ItemData.name);
                _pageView.UpdateContent(selectedView.ItemData, isUnlocked);
            }
        }
    }
} 
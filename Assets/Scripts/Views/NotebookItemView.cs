using Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Views
{
    public class NotebookItemView : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private Button button;
        [SerializeField] private Material lockedMaterial;
        [SerializeField] private GameObject selectorIcon;
        
        private GarbageItemData _itemData;
        private bool _isUnlocked;
        
        public GarbageItemData ItemData => _itemData;
        public bool IsUnlocked => _isUnlocked;
        
        public void Initialize(GarbageItemData itemData, bool isUnlocked, System.Action<NotebookItemView> onSelected)
        {
            _itemData = itemData;
            _isUnlocked = isUnlocked;
            
            UpdateVisuals();
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onSelected?.Invoke(this));
        }
        
        public void SetSelected(bool isSelected)
        {
            button.interactable = !isSelected;
            selectorIcon.SetActive(isSelected);
        }
        
        private void UpdateVisuals()
        {
            if (_isUnlocked)
            {
                itemImage.sprite = _itemData.ItemSprite;
                itemImage.material = null;
                itemNameText.text = _itemData.ItemName;
            }
            else
            {
                itemImage.sprite = _itemData.ItemSprite;
                itemImage.material = lockedMaterial;
                itemNameText.text = "???";
            }
        }
    }
} 
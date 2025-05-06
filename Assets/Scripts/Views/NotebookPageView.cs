using Data;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Views
{
    public class NotebookPageView : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private Material lockedMaterial;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        
        public void UpdateContent(GarbageItemData itemData, bool isUnlocked)
        {
            itemImage.sprite = itemData.ItemSprite;
            itemImage.material = null;
            
            if (isUnlocked)
            {
                itemImage.material = null;
                itemNameText.text = itemData.ItemName;
                itemDescriptionText.text = itemData.Description;
            }
            else
            {
                itemImage.material = lockedMaterial;
                itemNameText.text = "???";
                itemDescriptionText.text = "This item has not been discovered yet.";
            }
        }
    }
} 
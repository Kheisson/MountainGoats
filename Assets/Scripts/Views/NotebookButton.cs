using Core;
using Managers;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    [RequireComponent(typeof(Button))]
    public class NotebookButton : MonoBehaviour
    {
        private Button _button;
        private INotebookManager _notebookManager;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }
        
        private void Start()
        {
            _notebookManager = ServiceLocator.Instance.GetService<INotebookManager>();
        }
        
        private void OnButtonClicked()
        {
            if (_notebookManager == null)
            {
                MgLogger.LogError("NotebookManager not found.");
                return;
            }
            
            if (!_notebookManager.IsNotebookOpen)
            {
                _notebookManager.ShowNotebook();
            }
            else
            {
                _notebookManager.HideNotebook();
            }
        }
        
        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnButtonClicked);
            }
        }
    }
} 
using Services;

namespace Managers
{
    public interface INotebookManager : IService
    {
        void ShowNotebook();
        void HideNotebook();
        bool IsNotebookOpen { get; }
        void UpdateNotebook();
    }
} 
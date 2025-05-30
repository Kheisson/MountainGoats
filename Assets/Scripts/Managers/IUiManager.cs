using Controllers;
using Services;

namespace Managers
{
    public interface IUiManager : IService
    {
        public void ShowHudCanvas();
        public void HideHudCanvas();

        CurrencyController CurrencyController { get; }
    }
}
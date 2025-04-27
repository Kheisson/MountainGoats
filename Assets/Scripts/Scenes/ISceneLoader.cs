using System;
using Cysharp.Threading.Tasks;
using Services;

namespace Scenes
{
    public interface ISceneLoader : IService
    {
        UniTask LoadSceneAsync(string sceneName, bool showLoadingScreen = true);
        UniTask LoadSceneAsync(int sceneIndex, bool showLoadingScreen = true);
        UniTask UnloadSceneAsync(string sceneName, bool showLoadingScreen = true);
        UniTask UnloadSceneAsync(int sceneIndex, bool showLoadingScreen = true);
        event Action<string> SceneLoadedEvent;
        event Action<string> SceneUnloadedEvent;
    }
}
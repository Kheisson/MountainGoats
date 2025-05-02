using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using Spawning;

namespace GameManagement
{
    public class GameManager : BaseMonoBehaviour
    {
        private ISpawnerService _spawnerService;
        private EGameState _gameState;

        protected override HashSet<Type> RequiredServices { get; } = new HashSet<Type>()
        {
            typeof(ISpawnerService)
        };

        protected override void OnServicesInitialized()
        {
            _spawnerService = ServiceLocator.Instance.GetService<ISpawnerService>();
        }

        private async void Start()
        {
            _gameState = EGameState.Idle;
            await UniTask.WaitUntil(() => _isInitialized);
            _spawnerService.SpawnInitialGarbage();
        }

        private void OnEnable()
        {
            
        }
    }
}
using System;
using System.Collections.Generic;
using Core;
using EventsSystem;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameInput
{
    public class UnityInputService : MonoService, IInputService
    {
        [SerializeField] private InputActionAsset _inputActions;
        
        private const string PLAYER_ACTION_NAME = "Player";
        private const string MOVE_ACTION_NAME = "Move";
        private InputActionMap _playerActionMap;
        private InputAction _moveAction;
        private bool _isMoving;
        private IEventsSystemService _eventsSystemService;
        
        public override bool IsPersistent => false;

        public event Action<Vector2> OnMove;
        
        protected override HashSet<Type> RequiredServices => new HashSet<Type>
        {
            typeof(IEventsSystemService),
        };

        protected void Awake()
        {
            ServiceLocator.Instance.RegisterService<IInputService>(this);
            Initialize();
        }

        protected override void OnServicesInitialized()
        {
            _eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
            _eventsSystemService.Subscribe(ProjectConstants.Events.GAME_PAUSED, OnGamePause);
            _eventsSystemService.Subscribe(ProjectConstants.Events.GAME_RESUMED, OnGameResume);
        }

        public override void Initialize()
        {
            if (IsInitialized) return;
            
            base.Initialize();
            
            if (_inputActions == null)
            {
                MgLogger.LogError("Input Actions asset is not assigned!", this);
                return;
            }

            _playerActionMap = _inputActions.FindActionMap(PLAYER_ACTION_NAME);
            
            if (_playerActionMap == null)
            {
                MgLogger.LogError("Player Action Map not found in Input Actions!", this);
                return;
            }

            _moveAction = _playerActionMap.FindAction(MOVE_ACTION_NAME);
            
            if (_moveAction != null)
            {
                _moveAction.performed += OnMoveAction;
                _moveAction.canceled += OnMoveAction;
                _moveAction.started += OnMoveAction;
            }

            _playerActionMap.Enable();
            
            IsInitialized = true;
            MgLogger.Log("Input Service initialized", this);
        }

        public override void Shutdown()
        {
            if (_playerActionMap != null)
            {
                _playerActionMap.Disable();
                
                if (_moveAction != null)
                {
                    _moveAction.performed -= OnMoveAction;
                    _moveAction.canceled -= OnMoveAction;
                    _moveAction.started -= OnMoveAction;
                }
            }

            OnMove = null;
            
            base.Shutdown();
            
            ServiceLocator.Instance.UnregisterService<IInputService>();
            MgLogger.Log("Input Service shutdown", this);
        }

        private void Update()
        {
            if (!_isMoving || _moveAction == null) return;
            
            var moveInput = _moveAction.ReadValue<Vector2>();
            OnMove?.Invoke(moveInput);
        }

        private void OnMoveAction(InputAction.CallbackContext ctx)
        {
            var moveInput = ctx.ReadValue<Vector2>();
            OnMove?.Invoke(moveInput);
            
            switch (ctx.phase)
            {
                case InputActionPhase.Started:
                case InputActionPhase.Performed:
                    _isMoving = true;
                    break;
                case InputActionPhase.Canceled:
                    _isMoving = false;
                    OnMove?.Invoke(Vector2.zero);
                    break;
            }
        }
        
        private void OnGamePause()
        {
            _playerActionMap.Disable();
        }
        
        private void OnGameResume()
        {
            _playerActionMap.Enable();
        }
    }
} 
using System;
using System.Collections.Generic;
using Analytics;
using Cameras;
using UnityEngine;
using Services;
using Core;
using EventsSystem;
using GameInput;
using GarbageManagement;
using Stats;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Collider))]
public class HookController : BaseMonoBehaviour
{
    private IInputService _inputService;
    private ICameraService _cameraService;
    private IEventsSystemService _eventsSystemService;
    private IPlayerStatsService _playerStatsService;
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider;
    private AutoSizeCollider _autoSizeCollider;
    private Tweener _rotationTween;
    
    [SerializeField] private RopeSimulator2D_V2 _ropeSimulator2D;
    [SerializeField] private Transform waterStartTransform;
    [SerializeField] private float airGravityScale = 5f;
    [SerializeField] private float waterFallSpeed = 2f;
    [SerializeField] private float retractDuration = 1f;
    [SerializeField] private ParticleSystem splashFX;
    [SerializeField] private GameObject art;
    [SerializeField] private float rotationDuration = 0.2f;
    
    private bool isCast = false;
    private bool isInWater;
    private float _waterDepth;
    private Garbage _hookedGarbage;
    private float _currentTargetAngle;

    public bool IsReeling = false;

    public bool IsInWater => isInWater;
    
    protected override HashSet<Type> RequiredServices => new() 
    { 
        typeof(IInputService), 
        typeof(ICameraService), 
        typeof(IEventsSystemService),
        typeof(IPlayerStatsService),
    };

    protected override void Awake()
    {
        base.Awake();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        _autoSizeCollider = FindFirstObjectByType<AutoSizeCollider>();
        DOTween.SetTweensCapacity(1250, 312);
    }

    protected override void OnServicesInitialized()
    {
        _inputService = ServiceLocator.Instance.GetService<IInputService>();
        _cameraService = ServiceLocator.Instance.GetService<ICameraService>();
        _eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
        _playerStatsService = ServiceLocator.Instance.GetService<IPlayerStatsService>();
        _ = ServiceLocator.Instance.GetService<IEventsSystemService>();
        _eventsSystemService.Subscribe<GarbageHookedData>(ProjectConstants.Events.GARBAGE_HOOKED, HandleGarbageHooked);
        _eventsSystemService.Subscribe(ProjectConstants.Events.GAME_PAUSED, OnGamePause);
        _eventsSystemService.Subscribe(ProjectConstants.Events.GAME_RESUMED, OnGameResume);
        _inputService.OnMove += HandleInputMovement;
    }

    protected void FixedUpdate()
    {
        if (!_isInitialized) return;
        
        if (isInWater && !IsReeling)
        {
            HandleFall();
        }
    }
    
    private void HandleGarbageHooked(GarbageHookedData data)
    {
        if (_hookedGarbage != null) return;
        
        MgLogger.Log("Garbage hooked event received: " + data.Garbage.ItemData.ItemName);

        _hookedGarbage = data.Garbage;
        _hookedGarbage.transform.SetParent(transform);
        _hookedGarbage.transform.localPosition = Vector3.zero;

        _ropeSimulator2D.StartReeling();
    }
    
    private void OnGamePause()
    {
        _isInitialized = false;
    }
    
    private void OnGameResume()
    {
        _isInitialized = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Water")) return;
        
        isInWater = true;
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        splashFX.gameObject.SetActive(true);
        _cameraService?.SwitchCamera(ECamera.Hook, transform);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if (_inputService != null)
        {
            _inputService.OnMove -= HandleInputMovement;
        }
        
        _rotationTween?.Kill();
    }
    
    public void CastHook(Vector2 direction, float power)
    {
        if (isCast) return;

        var beforeThrowPosition = transform.position;
        WaitForFrame(() =>
        {
            _collider.enabled = true;
            
            var directionBeforeThrow = transform.position - beforeThrowPosition;
            directionBeforeThrow.Normalize();
            
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody2D.gravityScale = airGravityScale;
        
            // _rigidbody2D.AddForce(direction * hookCastPower * powerMultiplier, ForceMode2D.Impulse);
            _rigidbody2D.linearVelocity = direction * power;
            isCast = true;
        });
    }

    private void HandleFall()
    {
        _waterDepth = _rigidbody2D.position.y - waterFallSpeed * Time.deltaTime;
        _rigidbody2D.position = new Vector2(_rigidbody2D.position.x, _waterDepth);
    }

    private void TrySendingDepthReachedEvent(float newY)
    {
        // This is a placeholder for the actual event sending logic
        var roundedY = Mathf.RoundToInt(newY / 10) * 10;
        MgLogger.Log($"Sending event for Y: {Math.Abs(roundedY)}");
        var depthReachedEvent = new DepthReachedEvent(Math.Abs(roundedY));
        depthReachedEvent.TrySendEvent();
        _waterDepth = 0;
    }

    private void HandleInputMovement(Vector2 direction)
    {
        if (!isInWater) return;
        
        var newX = _rigidbody2D.position.x + direction.x * _playerStatsService.HorizontalSpeed * Time.deltaTime;
        var minX = _autoSizeCollider.ColliderMinMax().Item1;
        var maxX = _autoSizeCollider.ColliderMinMax().Item2;
        
        if (newX < minX)
        {
            newX = minX;
        }
        else if (newX > maxX)
        {
            newX = maxX;
        }
        
        _rigidbody2D.position = new Vector2(newX, _rigidbody2D.position.y);
        
        HandleRotation(direction);
    }

    private void HandleRotation(Vector2 direction)
    {
        float targetAngle;
        
        if (IsReeling)
        {
            var directionToHook = (Vector2)transform.position - Vector2.zero;
            targetAngle = Mathf.Atan2(directionToHook.y, directionToHook.x) * Mathf.Rad2Deg + 90f;
        }
        else
        {
            if (Mathf.Abs(direction.x) < 0.1f)
            {
                targetAngle = 0f;
            }
            else
            {
                targetAngle = direction.x > 0 ? 45 : -45;
            }
        }

        if (Mathf.Approximately(_currentTargetAngle, targetAngle)) return;
        
        _currentTargetAngle = targetAngle;
        _rotationTween?.Kill();
        _rotationTween = transform.DORotate(new Vector3(0, 0, targetAngle), rotationDuration)
            .SetEase(Ease.OutQuad);
    }

    public void ResetHookCast()
    {
        isCast = false;
        isInWater = false;
        splashFX.gameObject.SetActive(false);
        _cameraService?.SwitchCamera(ECamera.Player);
        _collider.enabled = false;
    }

    public void ResetHookPosition(Vector3 resetPos)
    {
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.position = resetPos; // without it, it has a race condition with the FixedUpdate for the physics calculationss
    }
    
    public void SetVisibility(bool value)
    {
        art.SetActive(value);
    }

    public void OnRetractComplete()
    {
        _eventsSystemService.Publish(ProjectConstants.Events.HOOK_RETRACTED);

        if (_hookedGarbage == null) return;
        
        _hookedGarbage.Collect();
        _hookedGarbage = null;
    }
}
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

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Collider))]
public class HookController : BaseMonoBehaviour
{
    private IInputService _inputService;
    private ICameraService _cameraService;
    private IEventsSystemService _eventsSystemService;
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider;
    private AutoSizeCollider _autoSizeCollider;
    
    [SerializeField] private Transform waterStartTransform;
    [SerializeField] private float hookCastPower = 4.5f;
    [SerializeField] private float airGravityScale = 5f;
    [SerializeField] private float waterFallSpeed = 2f;
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private ParticleSystem splashFX;
    [SerializeField] private GameObject art;
    
    private bool isCast = false;
    private bool isInWater;
    private float _waterDepth;
    
    protected override HashSet<Type> RequiredServices => new() { typeof(IInputService), typeof(ICameraService), typeof(IEventsSystemService) };

    protected override void Awake()
    {
        base.Awake();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        _autoSizeCollider = FindFirstObjectByType<AutoSizeCollider>();
    }

    protected override void OnServicesInitialized()
    {
        _inputService = ServiceLocator.Instance.GetService<IInputService>();
        _cameraService = ServiceLocator.Instance.GetService<ICameraService>();
        _eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
        _eventsSystemService.Subscribe<GarbageHookedData>(ProjectConstants.Events.GARBAGE_HOOKED, HandleGarbageHooked);
        _inputService.OnMove += HandleInputMovement;
    }

    protected void FixedUpdate()
    {
        if (!_isInitialized) return;
        
        if (isInWater)
        {
            HandleFall();
        }
    }
    
    private void HandleGarbageHooked(GarbageHookedData data)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Water")) return;
        
        isInWater = true;
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        splashFX.gameObject.SetActive(true);
        _cameraService?.SwitchCamera(ECamera.Hook, transform);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if (_inputService != null)
        {
            _inputService.OnMove -= HandleInputMovement;
        }
    }
    
    public void CastHook(Vector2 direction, float powerMultiplier)
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
            _rigidbody2D.linearVelocity = direction * hookCastPower * powerMultiplier;
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
        
        var newX = _rigidbody2D.position.x + direction.x * horizontalSpeed * Time.deltaTime;
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
}
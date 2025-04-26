using System;
using System.Collections.Generic;
using Cameras;
using UnityEngine;
using Services;
using Core;

[RequireComponent(typeof(Rigidbody2D))]
public class HookController : BaseMonoBehaviour
{
    private IInputService _inputService;
    private ICameraService _cameraService;
    private Rigidbody2D _rigidbody2D;
    
    [SerializeField] private Transform waterStartTransform;
    [SerializeField] private float hookCastPower = 4.5f;
    [SerializeField] private float airGravityScale = 5f;
    [SerializeField] private float waterFallSpeed = 2f;
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private ParticleSystem splashFX;
    
    private bool isCast = false;
    private bool isInWater;
    
    protected override HashSet<Type> RequiredServices => new() { typeof(IInputService), typeof(ICameraService) };

    protected override void Awake()
    {
        base.Awake();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected override void OnServicesInitialized()
    {
        _inputService = ServiceLocator.Instance.GetService<IInputService>();
        _cameraService = ServiceLocator.Instance.GetService<ICameraService>();
        _inputService.OnMove += HandleInputMovement;
    }

    protected void Update()
    {
        if (!_isInitialized) return;
        
        if (isInWater)
        {
            HandleFall();
        }
        else if (transform.position.y <= waterStartTransform.position.y)
        {
            isInWater = true;
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            splashFX.gameObject.SetActive(true);
            _cameraService?.SwitchCamera(ECamera.Hook, transform);
        }
        else if (transform.position.y > waterStartTransform.position.y && isInWater)
        {
            isInWater = false;
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            splashFX.gameObject.SetActive(false);
            _cameraService?.SwitchCamera(ECamera.Player);
        }
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
        
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.gravityScale = airGravityScale;
        
        _rigidbody2D.AddForce(direction * hookCastPower * powerMultiplier, ForceMode2D.Impulse);
        isCast = true;
    }

    private void HandleFall()
    {
        var newY = _rigidbody2D.position.y - waterFallSpeed * Time.deltaTime;
        _rigidbody2D.position = new Vector2(_rigidbody2D.position.x, newY);
    }

    private void HandleInputMovement(Vector2 direction)
    {
        if (!isInWater) return;
        
        var newX = _rigidbody2D.position.x + direction.x * horizontalSpeed * Time.deltaTime;
        _rigidbody2D.position = new Vector2(newX, _rigidbody2D.position.y);
    }
    
#if UNITY_EDITOR
    public void CheatReset()
    {
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        isCast = false;
        isInWater = false;
        splashFX.gameObject.SetActive(false);
        _cameraService?.SwitchCamera(ECamera.Player);
    }
#endif
}
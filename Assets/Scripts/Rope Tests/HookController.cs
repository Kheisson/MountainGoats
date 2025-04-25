using System;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Core;

[RequireComponent(typeof(Rigidbody2D))]
public class HookController : BaseMonoBehaviour
{
    private IInputService _inputService;
    private Rigidbody2D _rigidbody2D;
    
    [SerializeField] private bool isInWater;
    [SerializeField] private Transform waterStartTransform;
    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private ParticleSystem splashFX;
    
    protected override HashSet<Type> RequiredServices => new() { typeof(IInputService) };

    protected override void Awake()
    {
        base.Awake();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected override void OnServicesInitialized()
    {
        _inputService = ServiceLocator.Instance.GetService<IInputService>();
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

    private void HandleFall()
    {
        var newY = _rigidbody2D.position.y - fallSpeed * Time.deltaTime;
        _rigidbody2D.position = new Vector2(_rigidbody2D.position.x, newY);
    }

    private void HandleInputMovement(Vector2 direction)
    {
        if (!isInWater) return;
        
        var newX = _rigidbody2D.position.x + direction.x * horizontalSpeed * Time.deltaTime;
        _rigidbody2D.position = new Vector2(newX, _rigidbody2D.position.y);
    }
}
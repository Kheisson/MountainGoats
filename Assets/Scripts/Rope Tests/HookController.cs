using UnityEngine;
using Services;
using Core;

[RequireComponent(typeof(Rigidbody2D))]
public class HookController : MonoBehaviour
{
    private IInputService _inputService;
    private Rigidbody2D _rigidbody2D;
    
    [SerializeField] private bool isInWater;
    [SerializeField] private Transform waterStartTransform;
    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private ParticleSystem splashFX;
    
    private void Awake()
    {
        _inputService = ServiceLocator.Instance.GetService<IInputService>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
        if (_inputService == null)
        {
            MgLogger.LogWarning("InputService not initialized yet, waiting for initialization...", this);
            ServiceLocator.Instance.ServiceRegisteredEvent += HandleServiceRegistered;
        }
        else
        {
            SubscribeToInput();
        }
    }

    protected void Update()
    {
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

    private void HandleServiceRegistered(IService service)
    {
        if (service is not IInputService inputService) return;
        
        _inputService = inputService;
        SubscribeToInput();
        ServiceLocator.Instance.ServiceRegisteredEvent -= HandleServiceRegistered;
    }

    private void SubscribeToInput()
    {
        if (_inputService != null)
        {
            _inputService.OnMove += HandleInputMovement;
        }
    }

    private void OnDestroy()
    {
        if (_inputService != null)
        {
            _inputService.OnMove -= HandleInputMovement;
        }
        
        if (ServiceLocator.Instance != null)
        {
            ServiceLocator.Instance.ServiceRegisteredEvent -= HandleServiceRegistered;
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
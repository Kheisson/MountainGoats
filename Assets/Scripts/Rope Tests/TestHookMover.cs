using UnityEngine;
using Services;
using Core;

public class TestHookMover : MonoBehaviour
{
    private IInputService _inputService;
    public float fallSpeed = 2f;
    public float horizontalSpeed = 5f;

    private void Awake()
    {
        _inputService = ServiceLocator.Instance.GetService<IInputService>();
        
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
            _inputService.OnMove += HandleMove;
        }
    }

    private void OnDestroy()
    {
        if (_inputService != null)
        {
            _inputService.OnMove -= HandleMove;
        }
        
        if (ServiceLocator.Instance != null)
        {
            ServiceLocator.Instance.ServiceRegisteredEvent -= HandleServiceRegistered;
        }
    }

    private void HandleMove(Vector2 direction)
    {
        float newX = transform.position.x + direction.x * horizontalSpeed * Time.deltaTime;
        float newY = transform.position.y - fallSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}
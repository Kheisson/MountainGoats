using System;
using UnityEngine;

namespace Services
{
    public interface IInputService : IService
    {
        event Action<Vector2> OnMove;
    }
} 
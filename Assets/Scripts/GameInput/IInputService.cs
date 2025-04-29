using System;
using Services;
using UnityEngine;

namespace GameInput
{
    public interface IInputService : IService
    {
        event Action<Vector2> OnMove;
    }
} 
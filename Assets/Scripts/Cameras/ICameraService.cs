using Services;
using UnityEngine;

namespace Cameras
{
    public interface ICameraService : IService
    {
        void SwitchCamera(ECamera cameraType, Transform target = null);
    }
}
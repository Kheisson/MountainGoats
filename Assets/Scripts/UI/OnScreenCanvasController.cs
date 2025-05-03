#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using Core;
using UnityEngine;

namespace UI
{
    public class OnScreenCanvasController : MonoBehaviour
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int IsMobileDevice();
        #endif
        
        [SerializeField] private GameObject _mobileControls;
        [SerializeField] private bool _testMobileDevice = false;
        
        private bool _isMobile;

        private void Awake()
        {
#if UNITY_EDITOR
            if (_testMobileDevice)
            {
                _isMobile = true;
            }
#endif
            
#if UNITY_WEBGL && !UNITY_EDITOR
            _isMobile = IsMobileDevice() == 1;
#endif
            
            EnableMobileControls();
            MgLogger.Log("Mobile device detected: " + _isMobile);
        }

        private void EnableMobileControls()
        {
            _mobileControls?.SetActive(_isMobile);
        }
    }
}
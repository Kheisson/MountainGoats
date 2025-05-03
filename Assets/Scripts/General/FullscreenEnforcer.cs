#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
using Core;
#endif
using UnityEngine;

public class FullscreenEnforcer : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RequestFullscreen();
    
    [DllImport("__Internal")]
    private static extern int IsInFullscreen();
#endif
    
    public void ForceFullscreen()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestFullscreen();
        MgLogger.Log("Is in fullscreen: " + IsInFullscreen());
#endif
    }
}

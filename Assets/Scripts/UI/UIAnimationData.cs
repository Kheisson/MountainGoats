using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "NewUIAnimation", menuName = "UI/Animation Data")]
public class UIAnimationData : ScriptableObject
{
    public enum AnimationType
    {
        Wave,
        Bounce,
        Pulse,
        Shake,
        Rotate
    }

    public AnimationType animationType;
    public float duration = 1f;
    public Ease easeType = Ease.InOutSine;
    public LoopType loopType = LoopType.Yoyo;
    public int loops = -1; 

    // Wave specific
    public float waveAmplitude = 10f;
    public float waveHorizontalOffset = 5f;

    // Bounce specific
    public float bounceStrength = 0.2f;
    public int bounceVibrato = 10;

    // Pulse specific
    public float pulseScale = 1.2f;

    // Shake specific
    public float shakeStrength = 10f;
    public int shakeVibrato = 10;

    // Rotate specific
    public float rotationAngle = 15f;
} 
using System;
using Core;
using DG.Tweening;
using UnityEngine;

public class PlayerAnimationsController : BaseMonoBehaviour
{
    [SerializeField] private float hookCastPowerMultiplier = 1;
    [Header("Ready Parameters")]
    [SerializeField] private float readyRotationMultiplier;
    [SerializeField] private AnimationCurve rodCastReadyAnimationCurve;
    [SerializeField] private float rodCastReadyAnimationDuration;
    [Header("Set Parameters")]
    [SerializeField] private float rodSetAnimationDuration;
    [Header("Go Parameters")]
    [SerializeField] private float goRotationMultiplier;
    [SerializeField] private AnimationCurve rodCastGoAnimationCurve;
    [SerializeField] private float rodGoAnimationDuration;
    [Header("Reset Rotation Parameters")]
    [SerializeField] private AnimationCurve rodCastReturnAnimationCurve ;
    [SerializeField] private float rodCastReturnAnimationDuration ;
    
    
    // Before GPT
    // public void PlayHookCastAnimation(Transform rodPivot, bool castLeftSide, float normalizedThrowPower)
    // {
    //     var castPowerRotationModifier = normalizedThrowPower * hookCastPowerMultiplier;
    //     
    //     // Check additive vs multiply
    //     var finalReadyRotationAmount = readyRotationMultiplier * castPowerRotationModifier; 
    //     var readyTargetRotation = Quaternion.Euler(0, 0, rodPivot.eulerAngles.z * (castLeftSide ? -finalReadyRotationAmount : finalReadyRotationAmount));
    //     
    //     var finalGoRotationAmount = goRotationMultiplier * castPowerRotationModifier; 
    //     var goTargetRotation = Quaternion.Euler(0, 0, rodPivot.eulerAngles.z * (castLeftSide ? -finalGoRotationAmount : finalGoRotationAmount));
    //
    //     var hookCastSequence = DOTween.Sequence();
    //     hookCastSequence.Append(rodPivot
    //         .DORotate(readyTargetRotation.eulerAngles, rodCastReadyAnimationDuration)
    //         .SetEase(rodCastReadyAnimationCurve));
    //     hookCastSequence.AppendInterval(rodSetAnimationDuration);
    //     hookCastSequence.Append(rodPivot
    //         .DORotate(goTargetRotation.eulerAngles, rodCastReadyAnimationDuration)
    //         .SetEase(rodCastReadyAnimationCurve));
    //     hookCastSequence.Play();
    // }
    
    public void PlayHookCastAnimationSequence(Transform rod, Transform rodPivot, bool castLeftSide, float normalizedThrowPower, Action onAnimationComplete)
    {
        var pivot = rodPivot.transform.position;
        var target = rod;

        float castPowerRotationModifier = normalizedThrowPower * hookCastPowerMultiplier;

        float finalReadyRotationAmount = readyRotationMultiplier * castPowerRotationModifier;
        float finalGoRotationAmount = goRotationMultiplier * castPowerRotationModifier;

        float readyAngle = (castLeftSide ? -finalReadyRotationAmount : finalReadyRotationAmount);
        float goAngle = (castLeftSide ? -finalGoRotationAmount : finalGoRotationAmount);

        Vector3 startPos = target.position;
        Quaternion startRot = target.rotation;

        var seq = DOTween.Sequence();

        seq.Append(DOTween.To(() => 0f, angle =>
        {
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            Vector3 dir = startPos - pivot;
            target.position = pivot + rot * dir;
            target.rotation = startRot * Quaternion.Euler(0, 0, angle);
        }, readyAngle, rodCastReadyAnimationDuration).SetEase(rodCastReadyAnimationCurve));

        seq.AppendInterval(rodSetAnimationDuration);

        seq.Append(DOTween.To(() => readyAngle, angle =>
        {
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            Vector3 dir = startPos - pivot;
            target.position = pivot + rot * dir;
            target.rotation = startRot * Quaternion.Euler(0, 0, angle);
            onAnimationComplete?.Invoke();
        }, goAngle, rodCastReadyAnimationDuration).SetEase(rodCastReadyAnimationCurve));
        
        // // Return to original
        // seq.Append(DOTween.To(() => goAngle, angle =>
        // {
        //     Quaternion rot = Quaternion.Euler(0, 0, angle);
        //     Vector3 dir = startPos - pivot;
        //     target.position = pivot + rot * dir;
        //     target.rotation = startRot * Quaternion.Euler(0, 0, angle);
        // }, 0f, rodCastReturnAnimationDuration).SetEase(rodCastReturnAnimationCurve));
        // seq.OnKill(() =>
        // {
        //     // Animate back from current state to original
        //     DOTween.Kill(target); // prevent stacking return tweens if killed repeatedly
        //
        //     DOTween.To(() => 0f, angle =>
        //     {
        //         Quaternion rot = Quaternion.Euler(0, 0, angle);
        //         Vector3 dir = target.position - pivot;
        //         target.position = pivot + rot * dir;
        //         target.rotation = Quaternion.Lerp(target.rotation, startRot, angle / 1f);
        //     }, 1f, rodCastReturnAnimationDuration).SetEase(rodCastReturnAnimationCurve);
        // });
        seq.OnKill(() =>
        {
            DOTween.Kill(target); // Avoid overlapping tweens

            // Animate position and rotation back to original
            target.DOMove(startPos, rodCastReturnAnimationDuration).SetEase(rodCastReturnAnimationCurve);
            target.DORotateQuaternion(startRot, rodCastReturnAnimationDuration).SetEase(rodCastReturnAnimationCurve);
        });
        
        seq.Play();
    }

    public void Reset()
    {
        DOTween.Clear();
    }
}

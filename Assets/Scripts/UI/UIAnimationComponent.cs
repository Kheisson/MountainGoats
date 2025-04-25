using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
public class UIAnimationComponent : MonoBehaviour
{
    [SerializeField] private UIAnimationData animationData;
    [SerializeField] private bool playOnEnable = true;
    
    private RectTransform _rectTransform;
    private Tween _currentTween;
    private Vector2 _originalPosition;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
    }
    
    private void OnEnable()
    {
        if (animationData != null && playOnEnable)
        {
            PlayAnimation();
        }
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    public void PlayAnimation()
    {
        StopAnimation();

        if (animationData == null)
        {
            return;
        }

        switch (animationData.animationType)
        {
            case UIAnimationData.AnimationType.Wave:
                PlayWaveAnimation();
                break;
            case UIAnimationData.AnimationType.Bounce:
                PlayBounceAnimation();
                break;
            case UIAnimationData.AnimationType.Pulse:
                PlayPulseAnimation();
                break;
            case UIAnimationData.AnimationType.Shake:
                PlayShakeAnimation();
                break;
            case UIAnimationData.AnimationType.Rotate:
                PlayRotateAnimation();
                break;
        }
    }

    public void StopAnimation()
    {
        if (_currentTween != null)
        {
            _currentTween.Kill();
            _currentTween = null;
            _rectTransform.anchoredPosition = _originalPosition;
        }
    }

    private void PlayWaveAnimation()
    {
        Sequence waveSequence = DOTween.Sequence();

        // First half of the wave (up and right)
        waveSequence.Append(_rectTransform.DOAnchorPos(
            _originalPosition + new Vector2(animationData.waveHorizontalOffset * 0.5f, animationData.waveAmplitude),
            animationData.duration * 0.5f
        ).SetEase(animationData.easeType));

        // Second half of the wave (down and right)
        waveSequence.Append(_rectTransform.DOAnchorPos(
            _originalPosition + new Vector2(animationData.waveHorizontalOffset, 0),
            animationData.duration * 0.5f
        ).SetEase(animationData.easeType));

        // Return to start
        waveSequence.Append(_rectTransform.DOAnchorPos(
            _originalPosition,
            animationData.duration * 0.5f
        ).SetEase(animationData.easeType));

        waveSequence.SetLoops(animationData.loops, animationData.loopType);
        _currentTween = waveSequence;
    }

    private void PlayBounceAnimation()
    {
        _currentTween = _rectTransform.DOPunchScale(
            Vector3.one * animationData.bounceStrength,
            animationData.duration,
            animationData.bounceVibrato
        ).SetLoops(animationData.loops, animationData.loopType);
    }

    private void PlayPulseAnimation()
    {
        _currentTween = _rectTransform.DOScale(
            Vector3.one * animationData.pulseScale,
            animationData.duration
        ).SetEase(animationData.easeType)
         .SetLoops(animationData.loops, animationData.loopType);
    }

    private void PlayShakeAnimation()
    {
        _currentTween = _rectTransform.DOShakePosition(
            animationData.duration,
            animationData.shakeStrength,
            animationData.shakeVibrato
        ).SetLoops(animationData.loops, animationData.loopType);
    }

    private void PlayRotateAnimation()
    {
        _currentTween = _rectTransform.DORotate(
            new Vector3(0, 0, animationData.rotationAngle),
            animationData.duration
        ).SetEase(animationData.easeType)
         .SetLoops(animationData.loops, animationData.loopType);
    }
} 
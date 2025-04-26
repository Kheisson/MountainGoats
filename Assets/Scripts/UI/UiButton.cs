using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class UiButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
        [SerializeField] private ButtonAnimationPreset animationPreset;
        [SerializeField] private Button button;
        [SerializeField] private RectTransform buttonTransform;
        [SerializeField] private Graphic targetGraphic;

        private Vector3 _originalScale;
        private Color _originalColor;
        private Vector3 _originalPosition;
        private bool _isHovered;

        private void Awake()
        {
            if (button == null) button = GetComponent<Button>();

            if (buttonTransform == null) buttonTransform = GetComponent<RectTransform>();

            if (targetGraphic == null) targetGraphic = GetComponent<Graphic>() ?? button.targetGraphic;

            _originalScale = buttonTransform.localScale;
            _originalPosition = buttonTransform.anchoredPosition3D;

            if (targetGraphic != null) _originalColor = targetGraphic.color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.interactable || animationPreset == null) return;

            _isHovered = true;

            if (animationPreset.hoverAnimation.enabled)
            {
                ApplyAnimation(animationPreset.hoverAnimation);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!button.interactable || animationPreset == null) return;

            _isHovered = false;

            if (animationPreset.exitAnimation.enabled)
            {
                ApplyAnimation(animationPreset.exitAnimation);
            }
            else if (animationPreset.normalAnimation.enabled)
            {
                ApplyNormalState(animationPreset.normalAnimation);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.interactable || animationPreset == null) return;

            if (animationPreset.pressAnimation.enabled)
            {
                ApplyAnimation(animationPreset.pressAnimation);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!button.interactable || animationPreset == null) return;

            if (_isHovered && animationPreset.hoverAnimation.enabled)
            {
                ApplyAnimation(animationPreset.hoverAnimation);
            }
            else if (animationPreset.normalAnimation.enabled)
            {
                ApplyNormalState(animationPreset.normalAnimation);
            }
        }

        private void ApplyAnimation(ButtonAnimationPreset.AnimationSettings settings)
        {
            if (settings.animateScale)
            {
                buttonTransform.DOKill(true);
                buttonTransform.DOScale(settings.targetScale, settings.duration).SetEase(settings.easeType);
            }

            if (settings.animateColor && targetGraphic != null)
            {
                targetGraphic.DOKill(true);
                targetGraphic.DOColor(settings.targetColor, settings.duration).SetEase(settings.easeType);
            }

            if (!settings.animatePosition) return;

            buttonTransform.DOKill();
            buttonTransform.DOAnchorPos3D(_originalPosition + settings.positionOffset, settings.duration)
                .SetEase(settings.easeType);
        }

        private void ApplyNormalState(ButtonAnimationPreset.AnimationSettings settings)
        {
            if (settings.animateScale)
            {
                buttonTransform.DOKill(true);
                buttonTransform.DOScale(_originalScale, settings.duration).SetEase(settings.easeType);
            }

            if (settings.animateColor && targetGraphic != null)
            {
                targetGraphic.DOKill(true);
                targetGraphic.DOColor(_originalColor, settings.duration).SetEase(settings.easeType);
            }

            if (!settings.animatePosition) return;

            buttonTransform.DOKill();
            buttonTransform.DOAnchorPos3D(_originalPosition, settings.duration).SetEase(settings.easeType);
        }

        public void SetAnimationPreset(ButtonAnimationPreset newPreset)
        {
            animationPreset = newPreset;
        }

        private void OnDestroy()
        {
            buttonTransform.DOKill();

            if (targetGraphic != null)
            {
                targetGraphic.DOKill();
            }
        }
    }
}
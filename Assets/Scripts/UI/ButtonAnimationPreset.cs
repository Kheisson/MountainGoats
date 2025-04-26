using DG.Tweening;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "ButtonAnimationPreset", menuName = "UI/Button Animation Preset")]
    public class ButtonAnimationPreset : ScriptableObject
    {
        [System.Serializable]
        public class AnimationSettings
        {
            public bool enabled = true;
        
            [Header("Scale")]
            public bool animateScale = true;
            public Vector3 targetScale = Vector3.one;
        
            [Header("Color")]
            public bool animateColor = false;
            public Color targetColor = Color.white;
        
            [Header("Position")]
            public bool animatePosition = false;
            public Vector3 positionOffset = Vector3.zero;
        
            [Header("Timing")]
            public float duration = 0.2f;
            public Ease easeType = Ease.OutBack;
        }

        [Header("Hover Animations")]
        public AnimationSettings hoverAnimation = new AnimationSettings
        {
            targetScale = new Vector3(1.1f, 1.1f, 1f),
        };
    
        [Header("Press Animations")]
        public AnimationSettings pressAnimation = new AnimationSettings 
        {
            targetScale = new Vector3(0.95f, 0.95f, 1f),
            duration = 0.1f,
            easeType = Ease.OutCubic,
        };
    
        [Header("Exit Animations")]
        public AnimationSettings exitAnimation = new AnimationSettings 
        {
        };

        [Header("Normal State Animations")]
        public AnimationSettings normalAnimation = new AnimationSettings 
        {
            enabled = true,
        };
    }
}
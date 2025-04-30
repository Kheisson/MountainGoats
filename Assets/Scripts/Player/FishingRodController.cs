using System;
using Core;
using UnityEngine;

public class FishingRodController : BaseMonoBehaviour
{
    [SerializeField] private FishingRodGameObject[] fishingRodViewStates;
    
    private int currentIndex = -1;
    public Transform CurrentActiveHookPivot => fishingRodViewStates[currentIndex].HookPivot;
    
    public bool TrySetFishingRodStateAccordingToAngle(float normalizedCurrentAngle)
    {
        var index = Mathf.FloorToInt(normalizedCurrentAngle * fishingRodViewStates.Length);
        index = Mathf.Min(index, fishingRodViewStates.Length - 1);
        
        if (currentIndex == index) return false;
        currentIndex = index;
        
        foreach (var state in fishingRodViewStates)
        {
            state.Holder.SetActive(false);
        }

        fishingRodViewStates[index].Holder.SetActive(true);
        return true;
    }

    [Serializable]
    private class FishingRodGameObject
    {
        public GameObject Holder;
        public Transform HookPivot;
    }
}

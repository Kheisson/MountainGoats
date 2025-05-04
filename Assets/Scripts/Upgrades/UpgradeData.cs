using System;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrades
{
    [Serializable]
    public class UpgradeData
    {
        [field: SerializeField] public float Value { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Text Description { get; private set; }
    }
}
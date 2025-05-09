using System;
using UnityEngine;

namespace Upgrades
{
    [Serializable]
    public class UpgradeData
    {
        [field: SerializeReference, SubclassSelector] public IPlayerStatsModifier Modifier { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }
        [field: SerializeField] public Sprite PrePurchaseIcon { get; private set; }
        [field: SerializeField] public Sprite PostPurchaseIcon { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
    }
}
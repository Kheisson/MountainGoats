using System;
using UnityEngine;

namespace Upgrades
{
    [Serializable]
    public class UpgradeData : IUpgradeData
    {
        [field: SerializeReference, SubclassSelector] public IPlayerStatsModifier Modifier { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
    }
}
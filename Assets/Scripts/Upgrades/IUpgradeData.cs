using UnityEngine;

namespace Upgrades
{
    public interface IUpgradeData
    {
        IPlayerStatsModifier Modifier { get; }
        int Cost { get; }
        Sprite Icon { get; }
        string Description { get; }
    }
}
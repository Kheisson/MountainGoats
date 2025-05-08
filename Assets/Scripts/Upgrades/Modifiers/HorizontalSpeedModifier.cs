using System;
using Stats;

namespace Upgrades.Modifiers
{
    [Serializable]
    public class HorizontalSpeedModifier : BaseMultiplierModelModifier
    {
        protected override IPlayerStats ApplyModifierInternal(IPlayerStats baseModel)
        {
            return new PlayerStatsReadonly(
                baseModel.RopeLength,
                baseModel.HorizontalSpeed * multiplier,
                baseModel.LightIntensity,
                baseModel.EnergyCapacity);
        }
    }
}
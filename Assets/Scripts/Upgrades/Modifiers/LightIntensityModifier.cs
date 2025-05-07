using System;
using Stats;

namespace Upgrades.Modifiers
{
    [Serializable]
    public class LightIntensityModifier : BaseMultiplierModelModifier
    {
        protected override IPlayerStats ApplyModifierInternal(IPlayerStats baseModel)
        {
            return new PlayerStatsReadonly(
                baseModel.RopeLength,
                baseModel.HorizontalSpeed,
                baseModel.LightIntensity * multiplier,
                baseModel.EnergyCapacity);
        }
    }
}
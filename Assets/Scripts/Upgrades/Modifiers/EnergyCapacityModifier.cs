using System;
using Models;
using Stats;

namespace Upgrades.Modifiers
{
    [Serializable]
    public class EnergyCapacityModifier : BaseMultiplierModelModifier
    {
        protected override IPlayerStats ApplyModifierInternal(IPlayerStats baseModel)
        {
            return new PlayerStatsReadonly(
                baseModel.RopeLength,
                baseModel.HorizontalSpeed,
                baseModel.LightIntensity,
                baseModel.EnergyCapacity * multiplier);
        }
    }
}
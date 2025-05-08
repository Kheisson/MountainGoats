using System;
using Stats;

namespace Upgrades.Modifiers
{
    [Serializable]
    public class RopeLengthModifier : BaseMultiplierModelModifier
    {
        protected override IPlayerStats ApplyModifierInternal(IPlayerStats baseModel)
        {
            return new PlayerStatsReadonly(
                baseModel.RopeLength * multiplier, 
                baseModel.HorizontalSpeed,
                baseModel.LightIntensity, 
                baseModel.EnergyCapacity);
        }
    }
}
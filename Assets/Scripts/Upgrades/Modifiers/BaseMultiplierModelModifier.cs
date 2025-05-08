using Stats;
using UnityEngine;

namespace Upgrades
{
    public abstract class BaseMultiplierModelModifier : IPlayerStatsModifier
    {
        [SerializeField] protected float multiplier;
        
        public IPlayerStats ApplyModifier(IPlayerStats baseModel)
        {
            return ApplyModifierInternal(baseModel);
        }

        protected abstract IPlayerStats ApplyModifierInternal(IPlayerStats baseModel);
    }
}
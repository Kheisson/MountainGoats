using Models;
using Stats;

namespace Upgrades
{
    public interface IPlayerStatsModifier
    {
        IPlayerStats ApplyModifier(IPlayerStats baseModel);
    }
}
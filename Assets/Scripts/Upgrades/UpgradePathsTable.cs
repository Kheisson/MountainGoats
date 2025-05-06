using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Stats;
using UnityEngine;

namespace Upgrades
{
    [CreateAssetMenu(menuName = "Data/Upgrade Paths")]
    public class UpgradePathsTable : ScriptableObject
    {
        [field: SerializeField] 
        public SerializedDictionary<EUpgradeType, UpgradePath> UpgradePaths { get; private set; }

        public IPlayerStats ApplyUpgrade(IPlayerStats basePlayerStats, EUpgradeType upgradeType, int maxUpgradeIndex)
        {
            var upgradePath = UpgradePaths[upgradeType];
            
            basePlayerStats = upgradePath.AvailableUpgrades[maxUpgradeIndex].Modifier.ApplyModifier(basePlayerStats);

            return basePlayerStats;
        }
    }
}
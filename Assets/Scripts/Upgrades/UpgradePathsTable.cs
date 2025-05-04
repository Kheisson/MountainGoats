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

        public IPlayerStats ApplyUpgrade(IPlayerStats basePlayerStats, EUpgradeType upgradeType, List<int> upgradeIndices)
        {
            var upgradePath = UpgradePaths[upgradeType];

            if (upgradePath.isLinearUpgrade)
            {
                var maxUpgradeIndex = upgradeIndices.Max();
                basePlayerStats = upgradePath.AvailableUpgrades[maxUpgradeIndex].Modifier.ApplyModifier(basePlayerStats);
            }
            else
            {
                foreach (int upgradeIndex in upgradeIndices)
                {
                    basePlayerStats = upgradePath.AvailableUpgrades[upgradeIndex].Modifier.ApplyModifier(basePlayerStats);
                }
            }

            return basePlayerStats;
        }
    }
}
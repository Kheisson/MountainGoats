using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Upgrades
{
    [CreateAssetMenu(menuName = "Data/Upgrade Paths")]
    public class UpgradePaths : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<EUpgradeType, List<UpgradeData>> upgradePaths;
    }

}
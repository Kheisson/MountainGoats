using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(menuName = "Data/Upgrades model")]
    public class UpgradesModel : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<EUpgradeType, List<UpgradeData>> upgradePaths;
    }

}
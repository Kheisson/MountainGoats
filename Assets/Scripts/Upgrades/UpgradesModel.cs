using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Models
{
    [CreateAssetMenu(menuName = "Data/Upgrades model")]
    public class UpgradesModel : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<EUpgradeType, List<UpgradeData>> upgradePaths;
            
            
        [SerializeField] 
        private SerializableDictionary<string, string> nir;
    }

}
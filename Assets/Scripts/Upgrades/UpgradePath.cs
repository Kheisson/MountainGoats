using System;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrades
{
    [Serializable]
    public class UpgradePath
    {
        [field: SerializeField] public List<UpgradeData> AvailableUpgrades { get; private set; }
        [field: SerializeField] public bool isLinearUpgrade { get; private set; } = true;
    }
}
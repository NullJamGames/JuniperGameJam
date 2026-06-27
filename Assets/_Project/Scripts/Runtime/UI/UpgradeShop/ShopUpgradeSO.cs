using System;
using NJG.Runtime.Entity;
using UnityEngine;

namespace NJG.Runtime.UI
{
    [CreateAssetMenu(fileName = "ShopUpgrade", menuName = "NJG/Shop/Upgrade")]
    public class ShopUpgradeSO : ScriptableObject
    {
        [Serializable]
        public struct Upgrade
        {
            public BaseModifierSO[] Modifiers;
            public int Cost;
        }
        
        [field: SerializeField]
        public Upgrade[] Upgrades { get; private set; }
        [field: SerializeField]
        public string UpgradeName { get; private set; }
        [field: SerializeField]
        public Sprite UpgradeSprite { get; private set; }
        [field: SerializeField]
        public Color UpgradeColor { get; private set; } = Color.white;
    }
}
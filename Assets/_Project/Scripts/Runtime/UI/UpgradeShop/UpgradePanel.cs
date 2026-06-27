using System.Collections.Generic;
using NJG.Runtime.Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public class UpgradePanel : MenuPanel
    {
        [FoldoutGroup("Upgrade Shop"), SerializeField]
        private TextMeshProUGUI _playerCoinsText;
        [FoldoutGroup("Upgrade Shop"), SerializeField]
        private Button _backButton;
        [FoldoutGroup("Upgrade Shop"), SerializeField]
        private UpgradeItemVisual _upgradeItemPrefab;
        [FoldoutGroup("Upgrade Shop"), SerializeField]
        private RectTransform _upgradeItemContainer;
        [FoldoutGroup("Upgrade Shop"), SerializeField]
        private ShopUpgradeSO[] _shopUpgrades;
        
        private List<UpgradeItemVisual> _upgradeItems = new();
        private bool _wasInitialized;
        
        private void OnDestroy()
        {
            foreach (UpgradeItemVisual item in _upgradeItems)
            {
                item.OnUpgradePurchased -= OnUpgradePurchased;
            }
            
            _backButton.onClick.RemoveAllListeners();
        }
        
        public override void OnShow()
        {
            base.OnShow();
            
            if (!_wasInitialized)
            {
                InitializeUpgradeShop();
                _wasInitialized = true;
            }

            foreach (UpgradeItemVisual item in _upgradeItems)
                item.UpdateBuyableStates();
        
            _playerCoinsText.SetText($"Coins: {GameManager.Instance.PlayerCoins}");
        }
        
        private void InitializeUpgradeShop()
        {
            foreach (ShopUpgradeSO upgrade in _shopUpgrades)
            {
                UpgradeItemVisual item = Instantiate(_upgradeItemPrefab, _upgradeItemContainer);
                item.Init(upgrade);
                item.OnUpgradePurchased += OnUpgradePurchased;
                
                _upgradeItems.Add(item);
            }
            
            _backButton.onClick.AddListener(OnHide);
        }
        
        private void OnUpgradePurchased()
        {
            _playerCoinsText.SetText($"Coins: {GameManager.Instance.PlayerCoins}");
        }
    }
}
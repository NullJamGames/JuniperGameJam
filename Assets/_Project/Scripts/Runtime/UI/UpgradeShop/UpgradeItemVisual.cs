using System;
using NJG.Runtime.Entity;
using NJG.Runtime.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public class UpgradeItemVisual : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _upgradeNameText;
        [SerializeField]
        private TextMeshProUGUI _costText;
        [SerializeField]
        private Image _upgradeIcon;
        [SerializeField]
        private RectTransform _pipContainer;
        [SerializeField]
        private Image _upgradePipPrefab;
        [SerializeField]
        private Button _upgradeButton;
        
        private ShopUpgradeSO _shopUpgrade;
        private Image[] _pips;
        private int _currentUpgradeIndex;

        public event Action OnUpgradePurchased;

        private void OnDestroy()
        {
            _upgradeButton.onClick.RemoveAllListeners();
        }

        public void Init(ShopUpgradeSO shopUpgrade)
        {
            _shopUpgrade = shopUpgrade;
            _upgradeNameText.SetText(shopUpgrade.UpgradeName);
            _costText.SetText($"$ {shopUpgrade.Upgrades[0].Cost}");
            _upgradeIcon.sprite = shopUpgrade.UpgradeSprite;
            _upgradeIcon.color = shopUpgrade.UpgradeColor;
            _upgradeButton.onClick.AddListener(OnButton_Upgrade);
            
            _pips = new Image[shopUpgrade.Upgrades.Length];
            for (int x = 0; x < shopUpgrade.Upgrades.Length; x++)
            {
                Image pip = Instantiate(_upgradePipPrefab, _pipContainer);
                pip.color = Color.white;
                _pips[x] = pip;
            }
        }

        public void UpdateBuyableStates()
        {
            int playerCoins = GameManager.Instance.PlayerCoins;
            if (_currentUpgradeIndex >= _shopUpgrade.Upgrades.Length)
            {
                _costText.SetText("MAX");
                _costText.color = Color.gray;
                _upgradeButton.interactable = false;
                return;
            }

            if (playerCoins < _shopUpgrade.Upgrades[_currentUpgradeIndex].Cost)
            {
                _upgradeButton.interactable = false;
                _costText.color = Color.red;
                return;
            }
            
            _upgradeButton.interactable = true;
            _costText.color = Color.green;
        }

        private void OnButton_Upgrade()
        {
            if (GameManager.Instance.TryRemovePlayerCoins(_shopUpgrade.Upgrades[_currentUpgradeIndex].Cost))
            {
                foreach (BaseModifierSO modifier in _shopUpgrade.Upgrades[_currentUpgradeIndex].Modifiers)
                {
                    GameManager.Instance.AddShopModifier(modifier);
                }

                _pips[_currentUpgradeIndex].color = Color.green;
                _currentUpgradeIndex++;
                UpdateBuyableStates();
                OnUpgradePurchased?.Invoke();
            }
        }
    }
}
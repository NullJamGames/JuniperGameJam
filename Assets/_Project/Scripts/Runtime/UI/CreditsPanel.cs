using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public class CreditsPanel : MenuPanel
    {
        [FoldoutGroup("Dependencies"), SerializeField]
        private Button _backButton;

        private void Start()
        {
            _backButton.onClick.AddListener(OnButton_Back);
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();
        }

        private void OnButton_Back()
        {
            OnHide();
        }
    }
}
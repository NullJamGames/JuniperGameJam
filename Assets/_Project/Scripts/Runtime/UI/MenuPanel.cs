using NJG.Utilities;
using UnityEngine;

namespace NJG.Runtime.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MenuPanel : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        
        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        public virtual void OnShow()
        {
            Tools.ToggleVisibility(_canvasGroup, true);
        }
        
        public virtual void OnHide()
        {
            Tools.ToggleVisibility(_canvasGroup, false);
        }
    }
}
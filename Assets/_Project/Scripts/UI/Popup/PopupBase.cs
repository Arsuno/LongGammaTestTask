using System;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Popup
{
    public abstract class PopupBase : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private SimpleWindowAnimator _windowAnimator;

        public event Action<PopupBase> CloseClicked;
    
        private void Awake()
        {
            _closeButton.onClick.AddListener(InvokeClose);
        }

        public void Show()
        {
            _windowAnimator.PlayShow();
        }

        public void Hide()
        {
            _windowAnimator.PlayHide(() => Destroy(gameObject));
        }
    
        private void InvokeClose()
        {
            CloseClicked?.Invoke(this);
        }
    }
}
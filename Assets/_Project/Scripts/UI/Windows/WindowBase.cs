using System;
using _Project.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows
{
    public abstract class WindowBase : MonoBehaviour
    {
        [SerializeField] private WindowType _windowType;  
        [SerializeField] private WindowTag _windowTag;
        [SerializeField] private Button _closeButton;
        [SerializeField] private SimpleWindowAnimator _windowAnimator;
    
        public WindowType Type => _windowType;
        public WindowTag Tag => _windowTag;

        public event Action<WindowBase> CloseClicked;
    
        private void Awake()
        {
            _closeButton.onClick.AddListener(InvokeClose);
        }

        public void Show()
        {
            _windowAnimator.PlayShow();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _windowAnimator.PlayHide();
        }
    
        private void InvokeClose() => CloseClicked?.Invoke(this);
    }
}
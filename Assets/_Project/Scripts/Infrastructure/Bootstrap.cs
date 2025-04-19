using _Project.Scripts.UI;
using _Project.Scripts.UI.Popup;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using AudioSettings = _Project.Scripts.UI.Windows.AudioSettings;

namespace _Project.Scripts.Infrastructure
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private SlotsManager _slotsManager;
        
        private WindowManager _windowManager;
        private PopupManager _popupManager;

        private void Awake()
        {
            InitializeDependencies();
            _windowManager.LoadWindowsAndOpenFloating();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _popupManager.TryOpen<GroupInvite>(0, out _);
            }
        
            if (Input.GetKeyDown(KeyCode.X))
            {
                _windowManager.TryOpen<FriendsList>(1, out _);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                _windowManager.TryOpen<AudioSettings>(2, out _);
            }
        }

        private void InitializeDependencies()
        {
            _slotsManager.Initialize();
            IResourceLoader resourceLoader = new ResourceLoader();
            
            _windowManager = new WindowManager(resourceLoader, _slotsManager);
            _popupManager = new PopupManager(resourceLoader, _slotsManager);
        }
    }
}
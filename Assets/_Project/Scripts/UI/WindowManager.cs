using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.UI
{
    public class WindowManager
    {
        private readonly SlotsManager _slotsManager;
        private readonly Dictionary<WindowTag, WindowBase> _openedWindows = new();
        private readonly Dictionary<Type, WindowBase> _windowsInstances = new();
        private readonly Dictionary<Type, WindowBase> _windowPrefabs = new();
        private readonly Queue<(Type, int)> _windowsQueue = new();
        private readonly IResourceLoader _resourceLoader;

        private const int DEFAULT_SLOT_INDEX = 1;
        private const string WINDOWS_PATH = "Windows"; // лучше вынести или сделать доменную логику в IResourceLoader

        public WindowManager(IResourceLoader resourceLoader, SlotsManager slotsManager)
        {
            _resourceLoader = resourceLoader;
            _slotsManager = slotsManager;
        }
    
        public void LoadWindowsAndOpenFloating()
        {
            WindowBase[] windows = _resourceLoader.LoadAll<WindowBase>(WINDOWS_PATH);
            foreach (WindowBase windowPrefab in windows)
            {
                Type type = windowPrefab.GetType();
                _windowPrefabs.Add(type, windowPrefab);
                if (windowPrefab.Type == WindowType.Floating)
                {
                    TryOpen(type, DEFAULT_SLOT_INDEX, out _);
                }
            }
        }

        public bool TryOpen<T>(int slotIndex, out WindowBase openedWindow) where T : WindowBase
        {
            Type type = typeof(T);
            return TryOpen(type, slotIndex, out openedWindow);
        }
    
        public bool TryOpen(Type type, int slotIndex, out WindowBase window)
        {
            WindowBase windowToOpen = GetWindow(type);
            if (_openedWindows.TryGetValue(windowToOpen.Tag, out WindowBase openedWindow))
            {
                Close(openedWindow);
            }

            if (!TryPutIntoSlot(slotIndex, windowToOpen) && 
                !TryPutIntoSlot(slotIndex - 1, windowToOpen) && 
                !TryPutIntoSlot(slotIndex + 1, windowToOpen))
            {
                _windowsQueue.Enqueue((type, slotIndex));
                window = null;
                return false;
            }
        
            windowToOpen.CloseClicked += Close;
            windowToOpen.Show();
            window = windowToOpen;
            _openedWindows.Add(windowToOpen.Tag, windowToOpen);
            return true;
        }

        private void Close(WindowBase window)
        {
            window.CloseClicked -= Close;
            window.Hide();
            _slotsManager.FreeUpSlot(window.transform);
            _openedWindows.Remove(window.Tag);
        
            if (_windowsQueue.TryDequeue(out (Type type, int slotIndex) toOpen))
            {
                TryOpen(toOpen.type, toOpen.slotIndex, out _);
            }
        }

        private bool TryPutIntoSlot(int slotIndex, WindowBase windowToOpen)
        {
            return _slotsManager.TryPutIntoSlot(slotIndex, windowToOpen.transform);
        }

        private WindowBase GetWindow(Type type)
        {
            return _windowsInstances.TryGetValue(type, out WindowBase window) ? window : CreateWindow(type);
        }

        private WindowBase CreateWindow(Type type)
        {
            WindowBase prefab = _windowPrefabs[type];
            WindowBase instance = UnityEngine.Object.Instantiate(prefab);
            _windowsInstances.Add(type, instance);
            return instance;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Infrastructure;
using _Project.Scripts.UI.Popup;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class PopupManager
    {
        private readonly SlotsManager _slotsManager;
        private readonly IResourceLoader _resourceLoader;
        private readonly Dictionary<Type, PopupBase> _popupPrefabs = new();
        private readonly Dictionary<int, PopupBase> _openedPopups = new();
        private readonly Queue<(Type, int)> _popupsToOpen = new();

        private const string POPUP_PATH = "Popups/"; // лучше вынести или сделать доменную логику в IResourceLoader

        public PopupManager(IResourceLoader resourceLoader, SlotsManager slotsManager)
        {
            _resourceLoader = resourceLoader;
            _slotsManager = slotsManager;
        }

        public bool TryOpen<T>(int slotIndex, out PopupBase popup)
        {
            Type type = typeof(T);
            return TryOpen(type, slotIndex, out popup);
        }

        public bool TryOpen(Type type, int slotIndex, out PopupBase popup)
        {
            if (_openedPopups.ContainsKey(slotIndex))
            {
                _popupsToOpen.Enqueue((type, slotIndex));
                popup = null;
                return false;
            }
            
            if (_slotsManager.IsSlotTaken(slotIndex))
            {
                popup = null;
                return false;
            }
            
            popup = CreatePopup(type);
            popup.CloseClicked += Close;
            
            _slotsManager.TryPutIntoSlot(slotIndex, popup.transform);
            
            var mainBillboard = UnityEngine.Object.FindObjectOfType<BillboardCanvas>();
            if (mainBillboard != null)
            {
                popup.transform.localRotation = Quaternion.identity;
                popup.transform.rotation = mainBillboard.transform.rotation;
            }
            
            _openedPopups.Add(slotIndex, popup);
            return true;
        }

        private void Close(PopupBase popup)
        {
            popup.CloseClicked -= Close;
            KeyValuePair<int, PopupBase> item = _openedPopups.First(kvp => kvp.Value == popup);
            popup.Hide();
            _openedPopups.Remove(item.Key);
            _slotsManager.FreeUpSlot(popup.transform);

            if (_popupsToOpen.TryDequeue(out (Type type, int slotIndex) toOpen))
            {
                TryOpen(toOpen.type, toOpen.slotIndex, out _);
            }
        }

        private PopupBase GetPopup(Type type)
        {
            return CreatePopup(type);
        }

        private PopupBase CreatePopup(Type type)
        {
            PopupBase prefab = GetPrefab(type);
            PopupBase instance = UnityEngine.Object.Instantiate(prefab);
            instance.Show();
            return instance;
        }

        private PopupBase GetPrefab(Type type)
        {
            if (_popupPrefabs.TryGetValue(type, out PopupBase prefab))
            {
                return prefab;
            }

            string prefabName = type.Name;
            prefab = _resourceLoader.Load(POPUP_PATH + prefabName, typeof(PopupBase)) as PopupBase;
            _popupPrefabs.Add(type, prefab);
            return prefab;
        }
    }
}
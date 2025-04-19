using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class SlotsManager : MonoBehaviour
    {
        [SerializeField] private RectTransform[] _slots;
        
        private readonly Dictionary<Transform, int> _takenSlots = new();
        
        private bool[] _slotsTaken;

        public void Initialize()
        {
            _slotsTaken = new bool[_slots.Length];
        }

        public bool TryPutIntoSlot(int slotIndex, Transform windowTransform)
        {
            try
            {
                (RectTransform slotRect, bool slotTaken) = GetSlot(slotIndex);
                if (slotTaken)
                {
                    return false;
                }
            
                PutIntoSlot(slotRect, windowTransform, slotIndex);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        public bool IsSlotTaken(int slotIndex)
        {
            return GetSlot(slotIndex).slotTaken;
        }
    
        public void PutIntoSlot(RectTransform slotRect, Transform windowTransform, int slotIndex)
        {
            
            windowTransform.SetParent(slotRect);
            windowTransform.localPosition = Vector3.zero;
            windowTransform.localRotation = Quaternion.identity;

            _slotsTaken[slotIndex] = true;
            _takenSlots.Add(windowTransform, slotIndex);
        }

        public void FreeUpSlot(Transform transformToPut)
        {
            if (_takenSlots.TryGetValue(transformToPut, out int slotIndex))
            {
                _slotsTaken[slotIndex] = false;
                _takenSlots.Remove(transformToPut);
            }
            else
            {
                throw new ArgumentException($"No transform {transformToPut.name} in the slot list");
            }
        }

        private (RectTransform rectTransform, bool slotTaken) GetSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Length)
            {
                throw new ArgumentOutOfRangeException($"SlotIndex {slotIndex} is out of 0:{_slots.Length} range");
            }
        
            return (_slots[slotIndex], _slotsTaken[slotIndex]);
        }
    }
}
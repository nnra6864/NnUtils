using System;
using UnityEngine;

namespace NnUtils.Scripts
{
    [Serializable]
    public class ChanceItem
    {
        [SerializeField] private int _weight;
        public int Weight
        {
            get => _weight;
            set
            {
                if (_weight == value) return;
                _weight = value;
                OnChanceAmountChanged?.Invoke();
            }
        }
        public int ChanceAmountSilent { set => _weight = value; }
        [SerializeField] public float Chance;
        [SerializeField] [HideInInspector] public Vector2Int WeightRange;
        public Action OnChanceAmountChanged;
    }
    
    [Serializable]
    public class ChanceItem<T> : ChanceItem
    {
        [SerializeField] public T Item;
    }
}

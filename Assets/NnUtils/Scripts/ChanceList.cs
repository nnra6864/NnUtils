using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NnUtils.Scripts
{
    [Serializable]
    public class ChanceList<T> : IEnumerable<ChanceItem<T>>
    {
        [SerializeField] private List<ChanceItem<T>> _list;
        private int _maxChance;
        
        public void Add(ChanceItem<T> obj)
        {
            _list.Add(obj);
            CalculateChances();
            obj.OnChanceAmountChanged += CalculateChances;
        }
        
        public void Remove(ChanceItem<T> obj)
        {
            _list.Remove(obj);
            CalculateChances();
            obj.OnChanceAmountChanged += CalculateChances;
        }

        public void Clear()
        {
            _list.Clear();
            _maxChance = 0;
        }
        
        public ChanceItem<T> GetObject() => _list.First(x =>
        {
            if (_maxChance == 0) throw new Exception("Chance list is empty");
            var rand = Random.Range(0, _maxChance + 1);
            return x.WeightRange.x >= rand && x.WeightRange.y <= rand;
        });

        private void CalculateChances()
        {
            _maxChance = 0;
            _list.Sort((x, y) => x.Weight.CompareTo(y.Weight));

            foreach (var o in _list)
            {
                o.WeightRange = new(_maxChance + 1, _maxChance + o.Weight + 1);
                _maxChance = o.WeightRange.y + 1;
            }

            foreach (var o in _list)
            {
                o.Chance = (100f / _maxChance) * o.Weight;
            }
        }

        public IEnumerator<ChanceItem<T>> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
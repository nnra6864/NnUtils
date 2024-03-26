using System;
using System.Collections.Generic;

namespace NnUtils.Scripts
{
    public class NashSet<T> : HashSet<T>
    {
        public event Action<int> OnCountChanged;

        public new bool Add(T item)
        {
            bool added = base.Add(item);
            if (added)
            {
                OnCountChanged?.Invoke(this.Count);
            }

            return added;
        }

        public new bool Remove(T item)
        {
            bool removed = base.Remove(item);
            if (removed)
            {
                OnCountChanged?.Invoke(this.Count);
            }

            return removed;
        }

        public new void Clear()
        {
            base.Clear();
            OnCountChanged?.Invoke(this.Count);
        }
    }
}
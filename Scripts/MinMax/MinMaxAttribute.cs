// https://frarees.github.io/default-gist-license

using System;
using UnityEngine;

namespace Assets.NnUtils.Scripts.MinMax
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MinMaxAttribute : PropertyAttribute
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public bool DataFields { get; set; } = true;
        public bool FlexibleFields { get; set; } = true;
        public bool Bound { get; set; } = true;
        public bool Round { get; set; } = true;

        public MinMaxAttribute() : this(0, 1)
        {
        }

        public MinMaxAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
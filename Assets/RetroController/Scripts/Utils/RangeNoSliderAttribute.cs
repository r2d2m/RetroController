using System;
using UnityEngine;

namespace vnc.Utils
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class RangeNoSliderAttribute : PropertyAttribute
    {
        public float minimum;
        public float maximum;

        public RangeNoSliderAttribute(float minimum, float maximum)
        {
            this.minimum = minimum;
            this.maximum = maximum;
        }
    }
}

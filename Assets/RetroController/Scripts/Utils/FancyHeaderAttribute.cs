using System;
using UnityEngine;

namespace vnc.Utils
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class FancyHeaderAttribute : PropertyAttribute
    {
        public string Header = "";

        public FancyHeaderAttribute(string header)
        {
            this.Header = header;
        }
    }

}

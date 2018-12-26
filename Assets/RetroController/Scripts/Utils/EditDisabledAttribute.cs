using System;
using UnityEngine;

namespace vnc.Utils
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class EditDisabledAttribute : PropertyAttribute {

    }
}

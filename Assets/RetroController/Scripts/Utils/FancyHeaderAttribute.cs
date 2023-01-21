﻿using System;
using UnityEngine;

namespace epiplon.Utils
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class FancyHeaderAttribute : PropertyAttribute
    {
        public string Header = "";

        public FancyHeaderAttribute(string header)
        {
            Header = header;
        }
    }
}
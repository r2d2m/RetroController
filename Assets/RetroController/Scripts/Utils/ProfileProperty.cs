using System;
using UnityEngine;

namespace epiplon.Utils
{
    [Serializable]
    public abstract class ProfileProperty<T> where T : new()
    {
        [SerializeField]
        private T Value = new T();
        public T Runtime { get; protected set; }
        private bool isChanged = false;

        public void SetRuntime(T value) 
        { 
            Runtime = value;
            isChanged = true;
        }

        public void Reset() 
        { 
            Runtime = Value;
            isChanged = false;
        }

        public T Get() => isChanged ? Runtime : Value;

        /// <summary>
        /// Automatically converts the property type to T
        /// </summary>
        /// <param name="p">Property type</param>
        public static implicit operator T(ProfileProperty<T> p) => p.Get();
    }
}

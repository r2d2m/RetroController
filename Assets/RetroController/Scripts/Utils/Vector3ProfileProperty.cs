using UnityEngine;

namespace epiplon.Utils
{
    [System.Serializable]
    public class Vector3ProfileProperty : ProfileProperty<Vector3>
    {
        #region Overload
        public static Vector3 operator *(Vector3ProfileProperty v, FloatProfileProperty f)
        {
            return f.Get() * v.Get();
        }

        public static Vector3 operator *(FloatProfileProperty f, Vector3ProfileProperty v)
        {
            return f.Get() * v.Get();
        }

        public static Vector3 operator +(Vector3ProfileProperty v1, Vector3ProfileProperty v2)
        {
            return v1.Get() + v2.Get();
        }

        public static implicit operator Vector3(Vector3ProfileProperty p) => p.Get();
        #endregion
    }
}

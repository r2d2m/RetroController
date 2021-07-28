namespace epiplon.Utils
{
    [System.Serializable]
    public class FloatProfileProperty : ProfileProperty<float>
    {
        #region Overload
        public static float operator *(FloatProfileProperty property, float value)
        {
            return property.Get() * value;
        }

        public static float operator *(FloatProfileProperty p1, FloatProfileProperty p2)
        {
            return p1.Get() * p2.Get();
        }
        #endregion
    }
}

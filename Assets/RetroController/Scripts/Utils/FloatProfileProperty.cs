namespace epiplon.Utils
{
    [System.Serializable]
    public class FloatProfileProperty : ProfileProperty<float>
    {
        public FloatProfileProperty() : base() { }
        public FloatProfileProperty(float val) : base(val) { }

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

        public static float operator +(FloatProfileProperty p1, float p2)
        {
            return p1.Get() + p2;
        }
    }
}
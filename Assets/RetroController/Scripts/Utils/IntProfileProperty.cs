namespace epiplon.Utils
{
    [System.Serializable]
    public class IntProfileProperty : ProfileProperty<int>
    {
        #region Overload
        public static int operator *(IntProfileProperty property, int value)
        {
            return property.Get() * value;
        }

        public static int operator *(IntProfileProperty p1, IntProfileProperty p2)
        {
            return p1.Get() * p2.Get();
        }
        #endregion
    }
}

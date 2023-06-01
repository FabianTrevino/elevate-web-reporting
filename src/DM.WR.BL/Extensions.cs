namespace DM.WR.BL
{
    public static class Extensions
    {
        public static bool IsNullOrZero(this double? value)
        {
            return value == null || value == 0;
        }
    }
}
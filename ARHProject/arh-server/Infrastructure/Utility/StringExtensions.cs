namespace Infrastructure.Utility
{
    public static class StringExtensions
    {
        public static string UpperTrim(this string src)
        {
            if (!string.IsNullOrEmpty(src))
            {
                return src.Trim().ToUpper();
            }
            return null;
        }
    }
}
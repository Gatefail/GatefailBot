namespace GatefailBot.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength, bool withEllipsis = false)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength
                ? value
                : withEllipsis
                    ? value.Substring(0, maxLength - 1) + "…"
                    : value.Substring(0, maxLength);
        }
    }
}
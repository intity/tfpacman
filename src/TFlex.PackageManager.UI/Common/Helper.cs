namespace TFlex.PackageManager.Common
{
    internal static class Helper
    {
        /// <summary>
        /// Convert array to string.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToString(this string[] array, string separator)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
            {
                if (i != array.Length - 1)
                    result += array[i] + separator;
                else
                    result += array[i];
            }

            return result;
        }

        /// <summary>
        /// Convert chars array to string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToString(this char[] pattern, string separator)
        {
            string result = string.Empty;

            for (int i = 0; i < pattern.Length; i++)
            {
                char c = pattern[i];

                if (char.IsControl(c) || char.IsWhiteSpace(c) || c == '\0')
                    continue;

                if (i != pattern.Length - 1)
                    result += pattern[i] + separator;
                else
                    result += pattern[i];
            }
            return result;
        }

        /// <summary>
        /// Validating string on chars pattern.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <returns>
        /// Returns boolean value on the validation string result.
        /// </returns>
        public static bool IsValid(this string text, char[] pattern)
        {
            if (text.Length > 0 && text.IndexOfAny(pattern) >= 0)
                return false;

            return true;
        }
    }
}
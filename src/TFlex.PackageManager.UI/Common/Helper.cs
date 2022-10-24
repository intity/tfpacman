using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace TFlex.PackageManager.UI.Common
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
        /// Convert Icon to ImageSource.
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hbitmap = bitmap.GetHbitmap();
            ImageSource image = Imaging
                .CreateBitmapSourceFromHBitmap(
                hbitmap, 
                IntPtr.Zero, 
                Int32Rect.Empty, 
                BitmapSizeOptions.FromEmptyOptions());

            if (!NativeMethods.DeleteObject(hbitmap))
                throw new Win32Exception();

            return image;
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

        /// <summary>
        /// Validating a character by index in a string for a digit.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns>
        /// Returns boolean value on the validation string result.
        /// </returns>
        public static bool IsDigit(this string value, int index)
        {
            char[] chars = value.ToCharArray();
            return chars.Length > 0 && char.IsDigit(chars[index]);
        }

        /// <summary>
        /// Convert path to GUID.
        /// </summary>
        /// <param name="path">File name path.</param>
        /// <returns>Returns GUID to string format.</returns>
        public static string ToGUID(this string path)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(path));
                return new Guid(hash).ToString("D").ToUpper();
            }
        }
    }
}
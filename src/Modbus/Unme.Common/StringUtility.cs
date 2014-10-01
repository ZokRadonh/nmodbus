namespace Modbus.Unme.Common
{
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    public static class StringUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DoubleQuote(this string str)
        {
            return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", new object[] { str });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SingleQuote(this string str)
        {
            return string.Format(CultureInfo.InvariantCulture, "'{0}'", new object[] { str });
        }
    }
}
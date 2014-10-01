namespace Modbus.Unme.Common
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class DisposableUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        public static void Dispose<T>(ref T item) where T : class, IDisposable
        {
            if (item == null)
                return;

            item.Dispose();
            item = default(T);
        }
    }
}
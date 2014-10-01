namespace Modbus.Unme.Common
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class NullReferenceExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void IfNotNull<T>(this T element, Action<T> action)
        {
            if ((object)element == null)
                return;
            action(element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="func"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult IfNotNull<T, TResult>(this T element, Func<T, TResult> func)
        {
            if ((object)element == null)
                return default(TResult);

            return func(element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="func"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult IfNotNull<T, TResult>(this T element, Func<T, TResult> func, TResult defaultValue)
        {
            if ((object)element == null)
                return defaultValue;

            return func(element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="action"></param>
        /// <typeparam name="TTarget"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Is<TTarget>(this object element, Action<TTarget> action) where TTarget : class
        {
            if (action == null)
                throw new ArgumentNullException("action");

            (element as TTarget).IfNotNull(action);
        }
    }
}
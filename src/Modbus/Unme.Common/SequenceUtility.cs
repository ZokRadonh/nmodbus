namespace Modbus.Unme.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public static class SequenceUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="additionalItems"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, IEnumerable<T> second,
            params IEnumerable<T>[] additionalItems)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");
            if (additionalItems == null)
                throw new ArgumentNullException("additionalItems");

            var enumerable = first as T[] ?? first.ToArray();
            var result = Enumerable.Concat(enumerable, second);

            return additionalItems.Aggregate(result, Enumerable.Concat);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (action == null)
                throw new ArgumentNullException("action");
            foreach (T obj in source)
                action(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ForEachWithIndex<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (action == null)
                throw new ArgumentNullException("action");
            ForEach(WithIndex(source), x => action(x.Item2, x.Item1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="separator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> sequence, string separator)
        {
            return sequence.Join(separator, x => x.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="separator"></param>
        /// <param name="conversion"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Join<T>(this IEnumerable<T> sequence, string separator, Func<T, string> conversion)
        {
            if (separator == null)
                throw new ArgumentNullException("separator");
            if (conversion == null)
                throw new ArgumentNullException("conversion");

            return string.Join(separator, sequence.Select(conversion).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequence"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException("sequence");

            return new ReadOnlyCollection<T>(sequence.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> ToSequence<T>(this T element)
        {
            if ((object)element == null)
                throw new ArgumentNullException("element");

            return new[] { element };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="additional"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ToSequence<T>(T element, params T[] additional)
        {
            var sequence = element.ToSequence().ToArray();

            if (additional != null && additional.Length > 0)
                return Enumerable.Concat(sequence, additional);

            return sequence;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="size"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int startIndex, int size)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var enumerable = source as T[] ?? source.ToArray();
            int num = enumerable.Count();
            if (startIndex < 0 || num < startIndex)
                throw new ArgumentOutOfRangeException("startIndex");
            if (size < 0 || startIndex + size > num)
                throw new ArgumentOutOfRangeException("size");

            return enumerable.Skip(startIndex).Take(size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<Tuple<int, T>> WithIndex<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.Select((item, index) => Tuple.Create(index, item));
        }
    }
}
using System;
using System.Collections.Generic;

namespace NJG.Utilities
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Return string contains of rows. Each one contains one element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static string ToColumn<T>(this IEnumerable<T> enumerable)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var en in enumerable)
            {
                sb.AppendFormat("{0}\n", en);
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        
        /// <summary>
        ///     Performs an action on each element in the sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The sequence to iterate over.</param>
        /// <param name="action">The action to perform on each element.</param>
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (T item in sequence)
                action(item);
        }
    }
}
using System;
using System.Linq;
namespace UnityEngine
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns a random item from the specified array.
        /// </summary>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <param name="array">The array to select a random item from.</param>
        /// <returns>A random item from the array.</returns>
        /// <exception cref="ArgumentException">Thrown if the array is null or empty.</exception>
        public static T GetRandom<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Cannot select a random item from a null or empty array.", nameof(array));

            return array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Returns a random item from the specified array, excluding the specified exception element.
        /// </summary>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <param name="array">The array to select a random item from.</param>
        /// <param name="exceptionElement">The element to exclude from the random selection.</param>
        /// <returns>A random item from the array, excluding the exception element.</returns>
        /// <exception cref="ArgumentException">Thrown if the array is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if no eligible items are available.</exception>
        public static T RandomExcluding<T>(this T[] array, T exceptionElement)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Cannot select a random item from a null or empty array.", nameof(array));

            T[] eligibleItems = array.Except(new[] { exceptionElement }).ToArray();
            return eligibleItems.Length > 0 ? eligibleItems.GetRandom() : throw new InvalidOperationException("No eligible items available.");
        }

        /// <summary>
        /// Returns a random item from the specified array, excluding the specified exception elements.
        /// </summary>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <param name="array">The array to select a random item from.</param>
        /// <param name="exceptions">The elements to exclude from the random selection.</param>
        /// <returns>A random item from the array, excluding the exception elements.</returns>
        /// <exception cref="ArgumentException">Thrown if the array is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if no eligible items are available.</exception>
        public static T RandomExcluding<T>(this T[] array, T[] exceptions)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Cannot select a random item from a null or empty array.", nameof(array));

            T[] eligibleItems = array.Except(exceptions ?? Array.Empty<T>()).ToArray();
            return eligibleItems.Length > 0 ? eligibleItems.GetRandom() : throw new InvalidOperationException("No eligible items available.");
        }

        /// <summary>
        /// Creates a new array that is a copy of the original array.
        /// </summary>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <param name="array">The original array to be copied.</param>
        /// <returns>A new array that is a copy of the original array.</returns>
        public static T[] Clone<T>(this T[] array)
        {
            return array?.ToArray() ?? throw new ArgumentNullException(nameof(array));
        }

        /// <summary>
        /// Swaps two elements in the array at the specified indices.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to swap elements in.</param>
        /// <param name="indexA">The index of the first element to swap.</param>
        /// <param name="indexB">The index of the second element to swap.</param>
        public static void Swap<T>(this T[] array, int indexA, int indexB)
        {
            (array[indexA], array[indexB]) = (array[indexB], array[indexA]);
        }

        /// <summary>
        /// Shuffles the elements of the specified array in place using the Fisher-Yates shuffle algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to shuffle.</param>
        public static void Shuffle<T>(this T[] array)
        {
            for (int i = array.Length - 1; i > 1; i--)
            {
                int j = Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        /// <summary>
        /// Filters an array based on a predicate and returns a new filtered array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to filter.</param>
        /// <param name="predicate">The predicate to apply to each element.</param>
        /// <returns>A new array containing only the elements that satisfy the predicate.</returns>
        public static T[] Filter<T>(this T[] array, Predicate<T> predicate)
        {
            return array?.Where(item => predicate(item)).ToArray() ?? throw new ArgumentNullException(nameof(array));
        }
    }
}
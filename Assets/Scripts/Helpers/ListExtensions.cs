using System;
using System.Collections.Generic;
using System.Linq;
namespace UnityEngine
{
    public static class ListExtensions
    {
        /// <summary>
        /// Returns a random item from the specified list.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list to select a random item from.</param>
        /// <returns>A random item from the list.</returns>
        /// <exception cref="ArgumentException">Thrown if the list is null or empty.</exception>
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException("Cannot select a random item from a null or empty list.", nameof(list));

            return list[Random.Range(0, list.Count)];
        }
        
        /// <summary>
        /// Removes and returns a random item from the specified list.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list to remove a random item from.</param>
        /// <returns>The removed random item.</returns>
        /// <exception cref="ArgumentException">Thrown if the list is null or empty.</exception>
        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0) 
                throw new ArgumentException("Cannot remove a random item from a null or empty list", nameof(list));
            
            int index = Random.Range(0, list.Count);
            T item = list[index];
            list.RemoveAt(index);
            
            return item;
        }
        
        /// <summary>
        /// Returns a random item from the specified list, excluding the specified exception element.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list to select a random item from.</param>
        /// <param name="exceptionElement">The element to exclude from the random selection.</param>
        /// <returns>A random item from the list, excluding the exception element.</returns>
        /// <exception cref="ArgumentException">Thrown if the list is null or empty.</exception>
        public static T RandomExcluding<T>(this IList<T> list, T exceptionElement)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException("Cannot select a random item from a null or empty list.", nameof(list));
            
            IEnumerable<T> exception = new[] { exceptionElement, };
            List<T> eligibleItems = list.Except(exception).ToList();
            
            return eligibleItems.GetRandom();
        }
        
        /// <summary>
        /// Returns a random item from the specified list, excluding the specified exception elements.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list to select a random item from.</param>
        /// <param name="exceptions">The elements to exclude from the random selection.</param>
        /// <returns>A random item from the list, excluding the exception elements.</returns>
        /// <exception cref="ArgumentException">Thrown if the list is null or empty.</exception>
        public static T RandomExcluding<T>(this IList<T> list, IList<T> exceptions)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException("Cannot select a random item from a null or empty list.", nameof(list));
            
            if (exceptions == null)
                return list.GetRandom();
            
            List<T> eligibleItems = list.Except(exceptions).ToList();
            return eligibleItems.GetRandom();
        }
        
        /// <summary>
        /// Creates a new list that is a copy of the original list.
        /// </summary>
        /// <param name="list">The original list to be copied.</param>
        /// <returns>A new list that is a copy of the original list.</returns>
        public static List<T> Clone<T>(this IList<T> list)
        {
            List<T> newList = new List<T>();
            
            foreach (T item in list)
                newList.Add(item);

            return newList;
        }
        
        /// <summary>
        /// Swaps two elements in the list at the specified indices.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="indexA">The index of the first element.</param>
        /// <param name="indexB">The index of the second element.</param>
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        }
        
        /// <summary>
        /// Shuffles the elements of the specified list in place.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 1; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        
        /// <summary>
        /// Filters a collection based on a predicate and returns a new list
        /// containing the elements that match the specified condition.
        /// </summary>
        /// <param name="source">The collection to filter.</param>
        /// <param name="predicate">The condition that each element is tested against.</param>
        /// <returns>A new list containing elements that satisfy the predicate.</returns>
        public static IList<T> Filter<T>(this IList<T> source, Predicate<T> predicate)
        {
            List<T> list = new List<T>();
            
            foreach (T item in source)
                if (predicate(item))
                    list.Add(item);
            
            return list;
        }
    }
}
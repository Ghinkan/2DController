using System;
namespace UnityEngine
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns a random enum value of type T.
        /// </summary>
        /// <typeparam name="T">The type of enum to select a random value from.</typeparam>
        /// <returns>A random enum value of type T.</returns>
        public static T GetRandomEnum<T>()
        {
            Array enumArray = Enum.GetValues(typeof(T));
            T randomEnum = (T)enumArray.GetValue(Random.Range(0, enumArray.Length));
            return randomEnum;
        }
    }
}
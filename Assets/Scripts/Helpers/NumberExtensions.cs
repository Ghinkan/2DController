using System;
namespace UnityEngine
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Calculates the percentage of a part in relation to a whole.
        /// </summary>
        /// <param name="part">The part value.</param>
        /// <param name="whole">The whole value.</param>
        /// <returns>The percentage of the part in relation to the whole.</returns>
        public static float PercentageOf(this int part, int whole)
        {
            if (whole == 0) return 0; // Handling division by zero
            return (float) part / whole;
        }
        
        /// <summary>
        /// Checks if two float values are approximately equal.
        /// </summary>
        /// <param name="f1">The first float value.</param>
        /// <param name="f2">The second float value.</param>
        /// <returns>True if the values are approximately equal, false otherwise.</returns>
        public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);
        
        /// <summary>
        /// Checks if an integer is odd.
        /// </summary>
        /// <param name="i">The integer value.</param>
        /// <returns>True if the integer is odd, false otherwise.</returns>
        public static bool IsOdd(this int i)               => i % 2 == 1;
        
        /// <summary>
        /// Checks if an integer is even.
        /// </summary>
        /// <param name="i">The integer value.</param>
        /// <returns>True if the integer is even, false otherwise.</returns>
        public static bool IsEven(this int i)              => i % 2 == 0;
        
        /// <summary>
        /// Returns the maximum of a value and a minimum threshold.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum threshold.</param>
        /// <returns>The maximum of the value and the minimum threshold.</returns>
        public static int AtLeast(this int value, int min) => Mathf.Max(value, min);
        
        /// <summary>
        /// Returns the minimum of a value and a maximum threshold.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="max">The maximum threshold.</param>
        /// <returns>The minimum of the value and the maximum threshold.</returns>
        public static int AtMost(this int value, int max)  => Mathf.Min(value, max);
        
        /// <summary>
        /// Returns the maximum of a float value and a minimum threshold.
        /// </summary>
        /// <param name="value">The float value to check.</param>
        /// <param name="min">The minimum threshold.</param>
        /// <returns>The maximum of the float value and the minimum threshold.</returns>
        public static float AtLeast(this float value, float min) => Mathf.Max(value, min);
        
        /// <summary>
        /// Returns the minimum of a float value and a maximum threshold.
        /// </summary>
        /// <param name="value">The float value to check.</param>
        /// <param name="max">The maximum threshold.</param>
        /// <returns>The minimum of the float value and the maximum threshold.</returns>
        public static float AtMost(this float value, float max)  => Mathf.Min(value, max);
        
        /// <summary>
        /// Converts a linear value to a logarithmic scale.
        /// </summary>
        /// <param name="value">The linear value to convert.</param>
        /// <returns>The logarithmic equivalent of the input value.</returns>
        public static float ToLogarithmic(this float value)
        {
            return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        }
        
        /// <summary>
        /// Converts a linear fraction to a logarithmic scale.
        /// </summary>
        /// <param name="fraction">The linear fraction to convert, in the range [0, 1].</param>
        /// <returns>The logarithmic equivalent of the input fraction.</returns>
        public static float ToLogarithmicFraction(this float fraction)
        {
            return Mathf.Log10(1 + 9 * fraction) / Mathf.Log10(10);
        }
    }
}
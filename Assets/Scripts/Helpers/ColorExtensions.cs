namespace UnityEngine
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Sets the alpha component of the color.
        /// </summary>
        /// <param name="color">The original color.</param>
        /// <param name="alpha">The new alpha value.</param>
        /// <returns>A new color with the specified alpha value.</returns>
        public static Color SetAlpha(this Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

        /// <summary>
        /// Adds the RGBA components of two colors and clamps the result between 0 and 1.
        /// </summary>
        /// <param name="thisColor">The first color.</param>
        /// <param name="otherColor">The second color.</param>
        /// <returns>A new color that is the sum of the two colors, clamped between 0 and 1.</returns>
        public static Color Add(this Color thisColor, Color otherColor) => (thisColor + otherColor).Clamp01();

        /// <summary>
        /// Subtracts the RGBA components of one color from another and clamps the result between 0 and 1.
        /// </summary>
        /// <param name="thisColor">The first color.</param>
        /// <param name="otherColor">The second color.</param>
        /// <returns>A new color that is the difference of the two colors, clamped between 0 and 1.</returns>
        public static Color Subtract(this Color thisColor, Color otherColor) => (thisColor - otherColor).Clamp01();

        /// <summary>
        /// Clamps the RGBA components of the color between 0 and 1.
        /// </summary>
        /// <param name="color">The original color.</param>
        /// <returns>A new color with each component clamped between 0 and 1.</returns>
        public static Color Clamp01(this Color color)
        {
            return new Color {
                r = Mathf.Clamp01(color.r),
                g = Mathf.Clamp01(color.g),
                b = Mathf.Clamp01(color.b),
                a = Mathf.Clamp01(color.a)
            };
        }
        
        /// <summary>
        /// Blends two colors with a specified ratio.
        /// </summary>
        /// <param name="color1">The first color.</param>
        /// <param name="color2">The second color.</param>
        /// <param name="ratio">The blend ratio (0 to 1).</param>
        /// <returns>The blended color.</returns>
        public static Color Blend(this Color color1, Color color2, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            return new Color(
                color1.r * (1 - ratio) + color2.r * ratio,
                color1.g * (1 - ratio) + color2.g * ratio,
                color1.b * (1 - ratio) + color2.b * ratio,
                color1.a * (1 - ratio) + color2.a * ratio
            );
        }

        /// <summary>
        /// Inverts the color.
        /// </summary>
        /// <param name="color">The color to invert.</param>
        /// <returns>The inverted color.</returns>
        public static Color Invert(this Color color) => new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
    }
}
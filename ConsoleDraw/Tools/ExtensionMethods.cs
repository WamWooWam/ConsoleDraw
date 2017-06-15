using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw.Tools
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Clamps an <see cref="IComparable"/> to a minimum and maximum value.
        /// </summary>
        /// <typeparam name="T">The <see cref="IComparable"/> to clamp</typeparam>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>The clamped value.</returns>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}

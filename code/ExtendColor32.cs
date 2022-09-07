using System;
using UnityEngine;

namespace RealTimeStatistics
{
    /// <summary>
    /// extend Color32
    /// </summary>
    public static class ExtendColor32
    {
        /// <summary>
        /// multiply rgb colors (not alpha) by a value
        /// </summary>
        public static Color32 Multiply(this Color32 color, float multiplier)
        {
            return new Color32(Math.Max(Math.Min((byte)(color.r * multiplier), (byte)255), (byte)0),
                               Math.Max(Math.Min((byte)(color.g * multiplier), (byte)255), (byte)0),
                               Math.Max(Math.Min((byte)(color.b * multiplier), (byte)255), (byte)0),
                               color.a);
        }
    }
}

using System;
using System.IO;

namespace RealTimeStatistics
{
    /// <summary>
    /// extend BinaryWriter to write date and nullable values
    /// for date, write the year, month, day (i.e. the time is not included)
    /// for nullables, always write both the flag and the value, write zero if the value is null
    /// </summary>
    public static class ExtendBinaryWriter
    {
        /// <summary>
        /// Writes a date to the current stream and advances the stream position by 12 bytes.
        /// </summary>
        public static void Write(this BinaryWriter writer, DateTime value)
        {
            writer.Write(value.Year);
            writer.Write(value.Month);
            writer.Write(value.Day);
            writer.Write(value.Hour);
            writer.Write(value.Minute);
        }

        /// <summary>
        /// Writes a nullable four-byte signed integer to the current stream and advances the stream position by five bytes.
        /// </summary>
        public static void Write(this BinaryWriter writer, int? value)
        {
            writer.Write(value.HasValue);
            writer.Write(value ?? 0);
        }

        /// <summary>
        /// Writes a nullable four-byte unsigned integer to the current stream and advances the stream position by five bytes.
        /// </summary>
        public static void Write(this BinaryWriter writer, uint? value)
        {
            writer.Write(value.HasValue);
            writer.Write(value ?? 0);
        }

        /// <summary>
        /// Writes a nullable eight-byte signed integer to the current stream and advances the stream position by nine bytes.
        /// </summary>
        public static void Write(this BinaryWriter writer, long? value)
        {
            writer.Write(value.HasValue);
            writer.Write(value ?? 0);
        }

        /// <summary>
        /// Writes a nullable four-byte floating-point value to the current stream and advances the stream position by five bytes.
        /// </summary>
        public static void Write(this BinaryWriter writer, float? value)
        {
            writer.Write(value.HasValue);
            writer.Write(value ?? 0);
        }
    }
}

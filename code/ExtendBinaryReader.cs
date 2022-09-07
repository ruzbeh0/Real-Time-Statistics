using System;
using System.IO;

namespace RealTimeStatistics
{
    /// <summary>
    /// extend BinaryReader to read date and nullable values
    /// for date, read year, month, and day
    /// for nullables, always read both the flag and the value and return the value or null
    /// </summary>
    public static class ExtendBinaryReader
    {
        /// <summary>
        /// Reads a date from the current stream and advances the current position of the stream by 12 bytes.
        /// </summary>
        public static DateTime ReadDate(this BinaryReader reader)
        {
            int year  = reader.ReadInt32();
            int month = reader.ReadInt32();
            int day   = reader.ReadInt32();
            int hour = reader.ReadInt32();
            int minute = reader.ReadInt32();
            return new DateTime(year, month, day, hour, minute, 0);
            //return new DateTime(year, month, day);
        }

        /// <summary>
        /// Reads a nullable 4-byte signed integer from the current stream and advances the current position of the stream by five bytes.
        /// </summary>
        public static int? ReadNullableInt32(this BinaryReader reader)
        {
            bool hasValue = reader.ReadBoolean();
            int value = reader.ReadInt32();
            if (hasValue)
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Reads a nullable 4-byte unsigned integer from the current stream and advances the current position of the stream by five bytes.
        /// </summary>
        public static uint? ReadNullableUInt32(this BinaryReader reader)
        {
            bool hasValue = reader.ReadBoolean();
            uint value = reader.ReadUInt32();
            if (hasValue)
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Reads a nullable 8-byte signed integer from the current stream and advances the current position of the stream by nine bytes.
        /// </summary>
        public static long? ReadNullableInt64(this BinaryReader reader)
        {
            bool hasValue = reader.ReadBoolean();
            long value = reader.ReadInt64();
            if (hasValue)
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Reads a nullable 4-byte floating point value from the current stream and advances the current position of the stream by five bytes.
        /// </summary>
        public static float? ReadNullableSingle(this BinaryReader reader)
        {
            bool hasValue = reader.ReadBoolean();
            float value = reader.ReadSingle();
            if (hasValue)
            {
                return value;
            }
            return null;
        }
    }
}

using System;
using Okolni.Source.Common.ByteHelper;
using static Okolni.Source.Common.Enums;

namespace Okolni.Source.Common
{
    /// <summary>
    /// Helper class with static methods to help with arrays and other stuff
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Gets a subarray specified by the index and the length
        /// </summary>
        /// <param name="data">The array to get the subarray from</param>
        /// <param name="index">The index from where to get the subarray</param>
        /// <param name="length">The length of the subarray to extract</param>
        /// <typeparam name="T">The generic type of the array</typeparam>
        /// <returns>The sub array</returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Inserts an array into another array
        /// </summary>
        /// <param name="data">the array to insert</param>
        /// <param name="index">the index at which the array should be inserted</param>
        /// <param name="arrayToInsert">the array to insert</param>
        /// <typeparam name="T">The generic type of the array</typeparam>
        /// <returns>the original array with the inserted one</returns>
        public static T[] InsertArray<T>(this T[] data, int index, T[] arrayToInsert)
        {
            int maxlength = data.Length - index;
            if (arrayToInsert.Length > maxlength)
                arrayToInsert = arrayToInsert.SubArray(0, arrayToInsert.Length - maxlength);
            Array.Copy(arrayToInsert, 0, data, index, arrayToInsert.Length);
            return data;
        }

        public static void AppendToArray<T>(ref T[] data, T[] arrayToAppend)
        {
            int i = data.Length;
            Array.Resize(ref data, data.Length + arrayToAppend.Length);
            for (int j = 0; j < arrayToAppend.Length; j++)
            {
                data[i] = arrayToAppend[j];
                i++;
            }
        }

        public static int GetNextNullCharPosition(this byte[] data, int startindex)
        {
            for (; startindex < data.Length; startindex++)
            {
                if (data[startindex].Equals(0x00))
                    return startindex;
            }
            return -1;
        }

        public static IByteReader GetByteReader(this byte[] data)
        {
            return new ByteReader(data);
        }

        public static ServerType ToServerType(this byte data)
        {
            if (!Constants.ByteServerTypeMapping.TryGetValue(data, out var returnval))
                throw new ArgumentException("Given byte cannot be parsed");
            else
                return returnval;
        }

        public static Enums.Environment ToEnvironment(this byte data)
        {
            if (!Constants.ByteEnvironmentMapping.TryGetValue(data, out var returnval))
                throw new ArgumentException("Given byte cannot be parsed");
            else
                return returnval;
        }

        public static Visibility ToVisibility(this byte data)
        {
            if (!Constants.ByteVisibilityMapping.TryGetValue(data, out var returnval))
                throw new ArgumentException("Given byte cannot be parsed");
            else
                return returnval;
        }

        public static TheShipMode ToTheShipMode(this byte data)
        {
            if (!Constants.ByteTheShipModeMapping.TryGetValue(data, out var returnval))
                throw new ArgumentException("Given byte cannot be parsed");
            else
                return returnval;
        }
    }
}

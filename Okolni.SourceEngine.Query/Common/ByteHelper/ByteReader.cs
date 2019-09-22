using System;
using System.Text;

namespace Okolni.SourceEngine.Common.ByteHelper
{
    public class ByteReader : IByteReader
    {
        private int _iterator = 0;
        private byte[] _response;

        public byte[] Response
        {
            get
            {
                if(_response == null)
                    throw new ArgumentNullException("Response has to be set in order to get values from it");

                return _response;
            }
            set
            {
                _response = value;
                Iterator = 0;
            }
        }
        public int Iterator
        {
            get
            {
                return _iterator;
            }
            set
            {
                if (value < Response.Length && value >= 0)
                    _iterator = value;
                else if (value >= Response.Length && value >= 0)
                    _iterator = -1;
            }
        }
        public int Remaining
        {
            get
            {
                if (Iterator == -1)
                    return 0;
                return Response.Length - Iterator;
            }
        }

        public ByteReader(byte[] response)
        {
            Response = response;
        }

        /// <summary>
        /// Gets the ByteArray from the iterator with the specified length
        /// </summary>
        /// <param name="length">length to get</param>
        /// <returns>bytearray</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public byte[] GetBytes(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("The Length must be at least 1");
            if (Remaining < length)
                throw new ArgumentOutOfRangeException("Not Enough bytes left to read");

            var val = Response.SubArray(Iterator, length);
            Iterator += length;
            return val;
        }

        /// <summary>
        /// Gets the Byte Value at the current iterator position
        /// </summary>
        /// <returns>Byte value</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public byte GetByte()
        {
            if (Remaining < 1)
                throw new ArgumentOutOfRangeException("Not Enough bytes left to read");

            var val = Response[Iterator];
            Iterator++;
            return val;
        }

        /// <summary>
        /// Gets the Float Value at the current iterator position
        /// </summary>
        /// <returns>Float value</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public float GetFloat()
        {
            if(Remaining < 4)
                throw new ArgumentOutOfRangeException("Not Enough bytes left to read");

            float floatValue = BitConverter.ToSingle(Response, Iterator);
            Iterator += 4;

            return floatValue;
        }

        public short GetShort()
        {
            if (Remaining < 2)
                throw new ArgumentOutOfRangeException("Not Enough bytes left to read");

            short shortValue = BitConverter.ToInt16(Response, Iterator);
            Iterator += 2;

            return shortValue;
        }

        public int GetInt()
        {
            if(Remaining < 4)
                throw new ArgumentOutOfRangeException("Not Enough bytes left to read");

            int intValue = BitConverter.ToInt32(Response, Iterator);
            Iterator += 4;

            return intValue;
        }

        public uint GetLong()
        {
            if (Remaining < 4)
                throw new ArgumentOutOfRangeException("Not Enough bytes left to read");

            uint longValue = BitConverter.ToUInt32(Response, Iterator);
            Iterator += 4;

            return longValue;
        }

        public string GetString()
        {
            if (Remaining < 1)
                throw new ArgumentOutOfRangeException("Not Enough bytes left to read");

            int posNextNullChar = Response.GetNextNullCharPosition(Iterator);

            if (posNextNullChar == -1)
                throw new ArgumentOutOfRangeException("No valid string could be found in the remaining bytes");

            string stringValue = Encoding.UTF8.GetString(Response, Iterator, posNextNullChar - Iterator);

            Iterator = posNextNullChar + 1;

            return stringValue;
        }

        public byte[] GetRemaining(int? iterator = null)
        {
            if (iterator == null)
                iterator = Iterator;

            if (Remaining < 1)
                throw new ArgumentOutOfRangeException("Not Enough bytes left to read");

            return Response.SubArray((int)iterator, Remaining);
        }
    }
}

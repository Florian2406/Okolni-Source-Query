namespace Okolni.Source.Common.ByteHelper
{
    /// <summary>
    /// A byte reader to help read the byte response
    /// </summary>
    public interface IByteReader
    {
        /// <summary>
        /// The byte response
        /// </summary>
        byte[] Response { get; set; }

        /// <summary>
        /// The iterator position
        /// </summary>
        int Iterator { get; set; }

        /// <summary>
        /// The remaining bytes
        /// </summary>
        int Remaining { get; }


        /// <summary>
        /// Gets the byte array from the iterator to the length specified
        /// </summary>
        /// <param name="length">the length of the byte array to gather</param>
        /// <returns>The byte array extracted from the response</returns>
        byte[] GetBytes(int length);

        /// <summary>
        /// Gets the next byte in the response
        /// </summary>
        /// <returns>the byte gathered</returns>
        byte GetByte();

        /// <summary>
        /// Get's the remaining bytes as a string to the next null char
        /// </summary>
        /// <returns>the string</returns>
        string GetString();

        /// <summary>
        /// Extracts a short at the position of the iterator
        /// </summary>
        /// <returns>the short number</returns>
        short GetShort();


        /// <summary>
        /// Extracts a ushort at the position of the iterator (16 Bit unsigned int)
        /// </summary>
        /// <returns>the ushort number</returns>
        ushort GetUShort();


        /// <summary>
        /// Extracts an int at the position of the iterator
        /// </summary>
        /// <returns>the int number</returns>
        int GetInt();

        /// <summary>
        /// Extracts an uint at the position of the iterator
        /// </summary>
        /// <returns>the uint number</returns>
        uint GetUInt();

        /// <summary>
        /// Extracts a long at the position of the iterator
        /// </summary>
        /// <returns>the long number</returns>
        long GetLong();

        /// <summary>
        /// Extracts a Ulong at the position of the iterator (64 bit unsigned number)
        /// </summary>
        /// <returns>the Ulong number</returns>
        ulong GetULong();




        /// <summary>
        /// Extracts a float at the position of the iterator
        /// </summary>
        /// <returns>the float number</returns>
        float GetFloat();

        /// <summary>
        /// Returns the remaining bytes at the position of the iterator
        /// </summary>
        /// <param name="iterator">a custom iterator position</param>
        /// <returns>a byte array containing the remaining bytes in the response</returns>
        byte[] GetRemaining(int? iterator = null);
    }
}

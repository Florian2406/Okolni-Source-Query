namespace Okolni.SourceEngine.Common.ByteHelper
{
    public interface IByteReader
    {
        byte[] Response { get; set; }
        int Iterator { get; set; }
        int Remaining { get; }

        byte[] GetBytes(int length);
        byte GetByte();
        string GetString();
        short GetShort();
        int GetInt();
        uint GetLong();
        float GetFloat();
        byte[] GetRemaining(int? iterator = null);
    }
}

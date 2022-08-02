using System;

namespace Okolni.Source.Common
{
    public class MultiPacketResponse
    {
        public int Id { get; set; }
        public int Total { get; set; }
        public int Number { get; set; }
        public int Size { get; set; }
        public byte[] Payload { get; set; }
    }
}
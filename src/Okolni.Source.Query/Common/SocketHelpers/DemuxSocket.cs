using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Okolni.Source.Query.Common.SocketHelpers;

public class DemuxSocket : ISocket
{
    private readonly UDPDeMultiplexer _udpDeMultiplexer;
    private readonly Socket m_socket;

    public DemuxSocket(Socket socket, UDPDeMultiplexer udpDeMultiplexer)
    {
        m_socket = socket;
        _udpDeMultiplexer = udpDeMultiplexer;
    }

    /// <inheritdoc />
    public ValueTask<int> SendToAsync(
        ReadOnlyMemory<byte> buffer,
        SocketFlags socketFlags,
        EndPoint remoteEP,
        CancellationToken cancellationToken = default)
    {
#if DEBUG
        Console.WriteLine($"Sending  {Convert.ToBase64String(buffer.ToArray().Take(10).ToArray())} to {remoteEP}");
#endif
        return m_socket.SendToAsync(buffer, socketFlags, remoteEP, cancellationToken);
    }


    /// <inheritdoc />
    public ValueTask<SocketReceiveFromResult> ReceiveFromAsync(
        Memory<byte> buffer,
        SocketFlags socketFlags,
        EndPoint remoteEndPoint,
        CancellationToken cancellationToken = default)
    {
        return _udpDeMultiplexer.AddListener(buffer, socketFlags, remoteEndPoint, m_socket, cancellationToken);
    }
}
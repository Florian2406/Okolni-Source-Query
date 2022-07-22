using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Okolni.Source.Query.Common.SocketHelpers;

public interface ISocket
{
    /// <summary>Sends data to the specified remote host.</summary>
    /// <param name="buffer">The buffer for the data to send.</param>
    /// <param name="socketFlags">A bitwise combination of SocketFlags values that will be used when sending the data.</param>
    /// <param name="remoteEP">The remote host to which to send the data.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An asynchronous task that completes with the number of bytes sent.</returns>
    public ValueTask<int> SendToAsync(
        ReadOnlyMemory<byte> buffer,
        SocketFlags socketFlags,
        EndPoint remoteEP,
        CancellationToken cancellationToken = default);


    /// <summary>Receives data and returns the endpoint of the sending host.</summary>
    /// <param name="buffer">The buffer for the received data.</param>
    /// <param name="socketFlags">A bitwise combination of SocketFlags values that will be used when receiving the data.</param>
    /// <param name="remoteEndPoint">An endpoint of the same type as the endpoint of the remote host.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to signal the asynchronous operation should be
    ///     canceled.
    /// </param>
    /// <returns>
    ///     An asynchronous task that completes with a <see cref="T:System.Net.Sockets.SocketReceiveFromResult" />
    ///     containing the number of bytes received and the endpoint of the sending host.
    /// </returns>
    public ValueTask<SocketReceiveFromResult> ReceiveFromAsync(
        Memory<byte> buffer,
        SocketFlags socketFlags,
        EndPoint remoteEndPoint,
        CancellationToken cancellationToken = default);
}
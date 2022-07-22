using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Okolni.Source.Query.Common.SocketHelpers;
/*
 * This is really work in progress
 * The basic idea was just to use a single socket instead of potentially thousands (depending on how many queries you are making)... Since we are just sending out a few UDP Packets and getting a few back, we don't really need a dedicated socket or anything, really short life time & short packet lengths.
 * In order to make that work, we needed a way to still allow async (not callbacks or anything), receive the right responses to the right endpoints
 * I don't think there's any native way to do this with the .NET Socket API. I tried giving it a certain IP Endpoint in the ReceiveFromAsync method, but it would still get responses made for other endpoints
 * There's probably a better way to do this, and this is really messy right now, but it does seem to work without issue.
 */
public struct DemuxConnection
{

    public DemuxConnection(TaskCompletionSource<SocketReceiveFromResult> receiveFrom,
        Memory<byte> buffer, bool bufferDirty, bool generated)
    {
        ReceiveFrom = receiveFrom;
        CancellationToken = CancellationToken.None;
        MemoryBuffer = buffer;
        BufferDirty = bufferDirty;
        Generated = generated;
    }

    public DemuxConnection(TaskCompletionSource<SocketReceiveFromResult> receiveFrom,
        Memory<byte> buffer, bool bufferDirty, bool generated, CancellationToken cancellation)
    {
        ReceiveFrom = receiveFrom;
        CancellationToken = cancellation;
        MemoryBuffer = buffer;
        BufferDirty = bufferDirty;
        Generated = generated;
    }

    public TaskCompletionSource<SocketReceiveFromResult> ReceiveFrom;
    public CancellationToken CancellationToken;
    public Memory<byte> MemoryBuffer;
    public bool BufferDirty;
    public bool Generated;
}

public struct DemuxConnections
{
    public DemuxConnections()
    {
    }

    public DemuxConnections(DemuxConnection connection)
    {
        Connections.Enqueue(connection);
    }

    public ConcurrentQueue<DemuxConnection> Connections = new();
}

public class UDPDeMultiplexer
{
    private readonly Dictionary<EndPoint, DemuxConnections> Connections = new();


    public ValueTask<SocketReceiveFromResult> AddListener(Memory<byte> buffer,
        SocketFlags socketFlags,
        EndPoint remoteEndPoint,
        Socket socket,
        CancellationToken cancellationToken = default)
    {
        lock (Connections)
        {
            if (Connections.TryGetValue(remoteEndPoint, out var connections))
            {
                if (connections.Connections.TryDequeue(out var value))
                {
                    if (value.BufferDirty)
                    {
#if DEBUG
                        Console.WriteLine(
                            $"Found instantly for {remoteEndPoint}...exiting early... {Convert.ToBase64String(value.MemoryBuffer.ToArray().Take(10).ToArray())}");
#endif

                        value.BufferDirty = true;
                        value.MemoryBuffer.CopyTo(buffer);
                        return new ValueTask<SocketReceiveFromResult>(value.ReceiveFrom.Task);
                    }

                    var tcs = new TaskCompletionSource<SocketReceiveFromResult>();
                    Connections.Add(remoteEndPoint,
                        new DemuxConnections(new DemuxConnection(tcs, buffer, false, false, cancellationToken)));
#if DEBUG
                    Console.WriteLine($"New Listener: {remoteEndPoint} - {remoteEndPoint.Serialize()}");
#endif
                    return new ValueTask<SocketReceiveFromResult>(tcs.Task);
                }

                {
                    var tcs = new TaskCompletionSource<SocketReceiveFromResult>();
                    connections.Connections.Enqueue(new DemuxConnection(tcs, buffer, false, false, cancellationToken));
#if DEBUG
                    Console.WriteLine($"New Listener: {remoteEndPoint} - {remoteEndPoint.Serialize()}");
#endif
                    return new ValueTask<SocketReceiveFromResult>(tcs.Task);
                }
            }

            {
                var tcs = new TaskCompletionSource<SocketReceiveFromResult>();
                Connections.Add(remoteEndPoint,
                    new DemuxConnections(new DemuxConnection(tcs, buffer, false, false, cancellationToken)));
#if DEBUG
                Console.WriteLine($"New Listener: {remoteEndPoint} - {remoteEndPoint.Serialize()}");
#endif
                return new ValueTask<SocketReceiveFromResult>(tcs.Task);
            }
        }
    }


    public async Task Start(Socket socket, IPEndPoint endPoint)
    {
        try
        {
            while (true)
            {
                var newCancellationTokenSource = new CancellationTokenSource();

                Memory<byte> buffer = new byte[65527];
                var udpClientReceiveTask = socket.ReceiveFromAsync(buffer, SocketFlags.None, endPoint,
                    newCancellationTokenSource.Token).AsTask();

                if (await Task.WhenAny(udpClientReceiveTask, Task.Delay(500, newCancellationTokenSource.Token)) ==
                    udpClientReceiveTask)
                    try
                    {
                        newCancellationTokenSource.Cancel();
                        var udpClientReceive = udpClientReceiveTask.Result;
                        lock (Connections)
                        {
                            if (Connections.TryGetValue(udpClientReceive.RemoteEndPoint,
                                    out var connectionObj))
                            {
#if DEBUG
                                Console.WriteLine(
                                    $"Delivered packet to {udpClientReceive.RemoteEndPoint} - {Convert.ToBase64String(buffer.ToArray().Take(10).ToArray())}");
#endif
                                if (connectionObj.Connections.TryDequeue(out var demuxConnection) == false ||
                                    demuxConnection.BufferDirty)
                                {
                                    var tcs = new TaskCompletionSource<SocketReceiveFromResult>();
                                    tcs.SetResult(udpClientReceive);
                                    connectionObj.Connections.Enqueue(new DemuxConnection(tcs,
                                        buffer, true, true));
                                }
                                else
                                {
                                    demuxConnection.BufferDirty = true;
                                    buffer.CopyTo(demuxConnection.MemoryBuffer);
                                    demuxConnection.ReceiveFrom.SetResult(udpClientReceive);
                                }
                            }
                            else
                            {
#if DEBUG
                                Console.WriteLine(
                                    $"Found nothing listening... on {udpClientReceive.RemoteEndPoint}, {Connections.Count} - {Convert.ToBase64String(buffer.ToArray().Take(10).ToArray())}");
#endif
                                var tcs = new TaskCompletionSource<SocketReceiveFromResult>();
                                tcs.SetResult(udpClientReceive);
                                Connections.Add(udpClientReceive.RemoteEndPoint,
                                    new DemuxConnections(new DemuxConnection(tcs,  buffer, true,
                                        true)));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.WriteLine(
                            $"Unexpected issue handling packet from {(udpClientReceiveTask.IsCompleted ? udpClientReceiveTask.Result.RemoteEndPoint.ToString() ?? "Unknown" : "Unknown")}: " + ex);
#endif
                        throw;
                    }

                try
                {
                    lock (Connections)
                    {
                        foreach (var keyPair in Connections.Keys)
                        {
                            var demuxConnections = Connections[keyPair];
                            var connections = new List<DemuxConnection>();
                            while (demuxConnections.Connections.TryDequeue(out var value))
                                if (value.CancellationToken != CancellationToken.None &&
                                    value.CancellationToken.IsCancellationRequested)
                                    value.ReceiveFrom.TrySetException(
                                        new OperationCanceledException("Operation timed out"));
                                else
                                    connections.Add(value);

                            foreach (var connection in connections) demuxConnections.Connections.Enqueue(connection);
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine("Unexpected issue cleaning up connections..." + ex);
#endif
                }
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine("Unexpected issue:" + ex);
#endif
            throw;
        }
    }
}
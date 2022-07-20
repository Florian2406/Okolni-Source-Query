using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Okolni.Source.Query.Common
{
    public class UDPDeMultiplexer
    {
        ConcurrentDictionary<SocketAddress, TaskCompletionSource<SocketReceiveFromResult>> connections = new ConcurrentDictionary<SocketAddress, TaskCompletionSource<SocketReceiveFromResult>>();


        public Task<SocketReceiveFromResult> AddListener(IPEndPoint endpoint)
        {
            var tcs = new TaskCompletionSource<SocketReceiveFromResult>(); // Create the signal
            connections.TryAdd(endpoint.Serialize(), tcs);
            return tcs.Task;
        }


        public async Task Start(Socket socket, IPEndPoint endPoint)
        {
            while (true)
            {
                Memory<byte> buffer = new byte[65527];
                var udpClientReceive = await socket.ReceiveFromAsync(buffer, SocketFlags.None, endPoint);


                if (connections.TryGetValue(udpClientReceive.RemoteEndPoint.Serialize(), out var tcs))
                {
                    tcs.SetResult(udpClientReceive);
                }
                else
                {
                    tcs = new TaskCompletionSource<SocketReceiveFromResult>();
                    tcs.SetResult(udpClientReceive);
                    connections.TryAdd(udpClientReceive.RemoteEndPoint.Serialize(), tcs);
                }
            }
        }
    }
}

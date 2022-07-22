using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Okolni.Source.Common;
using Okolni.Source.Query.Common;
using Okolni.Source.Query.Common.SocketHelpers;
using Okolni.Source.Query.Responses;

namespace Okolni.Source.Query.Source;

public class QueryConnectionPool : IQueryConnectionPool, IDisposable
{
    private readonly UDPDeMultiplexer m_demultiplexer;
    private readonly Socket m_sharedsocket;

    private readonly DemuxSocket m_socket;

    public QueryConnectionPool()
    {
        m_sharedsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        SendTimeout = 1000;
        ReceiveTimeout = 1000;

        m_demultiplexer = new UDPDeMultiplexer();
        var ipendpoint = new IPEndPoint(IPAddress.Any, 0);
        m_sharedsocket.Bind(ipendpoint);
        _ = m_demultiplexer.Start(m_sharedsocket, ipendpoint).ContinueWith(t => Console.WriteLine(t.Exception),
                TaskContinuationOptions.OnlyOnFaulted)
            .ContinueWith(t => Console.WriteLine($"Worker exited safely {t.Status}"),
                TaskContinuationOptions.OnlyOnRanToCompletion);
        ;
        m_socket = new DemuxSocket(m_sharedsocket, m_demultiplexer);
    }

    public int SendTimeout { get; set; }
    public int ReceiveTimeout { get; set; }


    /// <summary>
    ///     Gets the servers general information
    /// </summary>
    /// <returns>InfoResponse containing all Infos</returns>
    /// <exception cref="SourceQueryException"></exception>
    public InfoResponse GetInfo(IPEndPoint endpoint, int maxRetries = 10)
    {
        return GetInfoAsync(endpoint, maxRetries).GetAwaiter().GetResult();
    }


    /// <summary>
    ///     Gets the servers general information
    /// </summary>
    /// <returns>InfoResponse containing all Infos</returns>
    /// <exception cref="SourceQueryException"></exception>
    public Task<InfoResponse> GetInfoAsync(IPEndPoint endpoint, int maxRetries = 10)
    {
        return QueryHelper.GetInfoAsync(endpoint, m_socket, SendTimeout, ReceiveTimeout, maxRetries);
    }


    /// <summary>
    ///     Gets all active players on a server
    /// </summary>
    /// <returns>PlayerResponse containing all players </returns>
    /// <exception cref="SourceQueryException"></exception>
    public PlayerResponse GetPlayers(IPEndPoint endpoint, int maxRetries = 10)
    {
        return GetPlayersAsync(endpoint, maxRetries).GetAwaiter().GetResult();
    }


    /// <summary>
    ///     Gets all active players on a server
    /// </summary>
    /// <returns>PlayerResponse containing all players </returns>
    /// <exception cref="SourceQueryException"></exception>
    public Task<PlayerResponse> GetPlayersAsync(IPEndPoint endpoint, int maxRetries = 10)
    {
        return QueryHelper.GetPlayersAsync(endpoint, m_socket, SendTimeout, ReceiveTimeout, maxRetries);
    }


    /// <summary>
    ///     Gets the rules of the server
    /// </summary>
    /// <returns>RuleResponse containing all rules as a Dictionary</returns>
    /// <exception cref="SourceQueryException"></exception>
    public RuleResponse GetRules(IPEndPoint endpoint, int maxRetries = 10)
    {
        return GetRulesAsync(endpoint, maxRetries).GetAwaiter().GetResult();
    }


    /// <summary>
    ///     Gets the rules of the server
    /// </summary>
    /// <returns>RuleResponse containing all rules as a Dictionary</returns>
    /// <exception cref="SourceQueryException"></exception>
    public Task<RuleResponse> GetRulesAsync(IPEndPoint endpoint, int maxRetries = 10)
    {
        return QueryHelper.GetRulesAsync(endpoint, m_socket, SendTimeout, ReceiveTimeout, maxRetries);
    }

    public void Dispose()
    {
        m_sharedsocket?.Dispose();
    }
}
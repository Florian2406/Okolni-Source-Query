using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Okolni.Source.Common;
using Okolni.Source.Common.ByteHelper;
using Okolni.Source.Query.Common;
using Okolni.Source.Query.Responses;

namespace Okolni.Source.Query.Source
{
    public class QueryConnectionPool : IQueryConnectionPool, IDisposable
    {
        private Socket m_sharedsocket;

        private UDPDeMultiplexer m_demultiplexer;

        public QueryConnectionPool()
        {
            m_sharedsocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            SendTimeout = 1000;
            ReceiveTimeout = 1000;
            m_demultiplexer = new UDPDeMultiplexer();
            _ = m_demultiplexer.Start(m_sharedsocket, new IPEndPoint(IPAddress.Any, 0));
        }

        public int SendTimeout { get; set; }
        public int ReceiveTimeout { get; set; }




        /// <inheritdoc />
        public void Setup()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Connect(int timeoutMiliSec)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Connect()
        {
            Connect(5000);
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (m_sharedsocket != null)
            {
                if (m_sharedsocket != null && m_sharedsocket.Connected)
                {
                    m_sharedsocket.Disconnect(false);
                }
                m_sharedsocket.Dispose();
            }
        }





        /// <summary>
        /// Gets the servers general information
        /// </summary>
        /// <returns>InfoResponse containing all Infos</returns>
        /// <exception cref="SourceQueryException"></exception>
        public InfoResponse GetInfo(IPEndPoint endpoint, int maxRetries = 10) => GetInfoAsync(endpoint, maxRetries).GetAwaiter().GetResult();


        /// <summary>
        /// Gets the servers general information
        /// </summary>
        /// <returns>InfoResponse containing all Infos</returns>
        /// <exception cref="SourceQueryException"></exception>
        public Task<InfoResponse> GetInfoAsync(IPEndPoint endpoint, int maxRetries = 10) =>
            QueryHelper.GetInfoAsync(endpoint, m_sharedsocket, SendTimeout, ReceiveTimeout, maxRetries);


        /// <summary>
        /// Gets all active players on a server
        /// </summary>
        /// <returns>PlayerResponse containing all players </returns>
        /// <exception cref="SourceQueryException"></exception>
        public PlayerResponse GetPlayers(IPEndPoint endpoint, int maxRetries = 10) => GetPlayersAsync(endpoint, maxRetries).GetAwaiter().GetResult();



        /// <summary>
        /// Gets all active players on a server
        /// </summary>
        /// <returns>PlayerResponse containing all players </returns>
        /// <exception cref="SourceQueryException"></exception>
        /// 
        public Task<PlayerResponse> GetPlayersAsync(IPEndPoint endpoint, int maxRetries = 10) => QueryHelper.GetPlayersAsync(endpoint, m_sharedsocket, SendTimeout, ReceiveTimeout, maxRetries);



        /// <summary>
        /// Gets the rules of the server
        /// </summary>
        /// <returns>RuleResponse containing all rules as a Dictionary</returns>
        /// <exception cref="SourceQueryException"></exception>
        public RuleResponse GetRules(IPEndPoint endpoint, int maxRetries = 10) => GetRulesAsync(endpoint, maxRetries).GetAwaiter().GetResult();


        /// <summary>
        /// Gets the rules of the server
        /// </summary>
        /// <returns>RuleResponse containing all rules as a Dictionary</returns>
        /// <exception cref="SourceQueryException"></exception>
        public Task<RuleResponse> GetRulesAsync(IPEndPoint endpoint, int maxRetries = 10) =>
            QueryHelper.GetRulesAsync(endpoint, m_sharedsocket, SendTimeout, ReceiveTimeout, maxRetries);

        public void Dispose()
        {
            m_sharedsocket?.Dispose();
        }
    }
}

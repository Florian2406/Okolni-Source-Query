using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Okolni.Source.Common;
using Okolni.Source.Common.ByteHelper;
using Okolni.Source.Query.Common;
using Okolni.Source.Query.Responses;

namespace Okolni.Source.Query
{
    public class QueryConnection : IQueryConnection, IDisposable
    {
        private Socket m_socket;       
        private IPEndPoint m_endPoint;

        public int SendTimeout { get; set; }
        public int ReceiveTimeout { get; set; }


        /// <inheritdoc />
        public string Host
        {
            get;
            set;
        } = "127.0.0.1";

        /// <inheritdoc />
        public int Port
        {
            get;
            set;
        } = 27015;

        public QueryConnection()
        {
            SendTimeout = 1000;
            ReceiveTimeout = 1000;
        }



        /// <inheritdoc />
        public void Setup()
        {
            if (string.IsNullOrEmpty(Host))
                throw new ArgumentNullException(nameof(Host), "A Host must be specified");
            if (Port < 0x0 || Port > 0xFFFF)
                throw new ArgumentOutOfRangeException(nameof(Port), "A Valid Port has to be specified (between 0x0000 and 0xFFFF)");

            if (m_socket != null || (m_socket != null && m_socket.Connected))
                return;

            m_socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            m_endPoint = new IPEndPoint(IPAddress.Parse(Host), Port);
        }

        /// <inheritdoc />
        public void Connect(int timeoutMiliSec)
        {
            if (string.IsNullOrEmpty(Host))
                throw new ArgumentNullException(nameof(Host), "A Host must be specified");
            if (Port < 0x0 || Port > 0xFFFF)
                throw new ArgumentOutOfRangeException(nameof(Port), "A Valid Port has to be specified (between 0x0000 and 0xFFFF)");

            if (m_socket != null || (m_socket != null && m_socket.Connected))
                return;

            m_socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            m_endPoint = new IPEndPoint(IPAddress.Parse(Host), Port);
            m_socket.Connect(m_endPoint);
            // Note: The timeout does nothing now because SendTimeout and Receive timeout only affect the sync Receive/Send methods..
            m_socket.SendTimeout = timeoutMiliSec;
            m_socket.ReceiveTimeout = timeoutMiliSec;
        }

        /// <inheritdoc />
        public void Connect()
        {
            Connect(5000);
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (m_socket != null)
            {
                if (m_socket != null && m_socket.Connected)
                {
                    m_socket.Disconnect(false);
                }
                m_socket.Dispose();
            }
        }





        /// <summary>
        /// Gets the servers general information
        /// </summary>
        /// <returns>InfoResponse containing all Infos</returns>
        /// <exception cref="SourceQueryException"></exception>
        public InfoResponse GetInfo(int maxRetries = 10) => GetInfoAsync().GetAwaiter().GetResult();


        /// <summary>
        /// Gets the servers general information
        /// </summary>
        /// <returns>InfoResponse containing all Infos</returns>
        /// <exception cref="SourceQueryException"></exception>
        public Task<InfoResponse> GetInfoAsync(int maxRetries = 10) =>
            QueryHelper.GetInfoAsync(m_endPoint, m_socket, SendTimeout, ReceiveTimeout, maxRetries);


        /// <summary>
        /// Gets all active players on a server
        /// </summary>
        /// <returns>PlayerResponse containing all players </returns>
        /// <exception cref="SourceQueryException"></exception>
        public PlayerResponse GetPlayers(int maxRetries = 10) => GetPlayersAsync().GetAwaiter().GetResult();



        /// <summary>
        /// Gets all active players on a server
        /// </summary>
        /// <returns>PlayerResponse containing all players </returns>
        /// <exception cref="SourceQueryException"></exception>
        /// 
        public Task<PlayerResponse> GetPlayersAsync(int maxRetries = 10) => QueryHelper.GetPlayersAsync(m_endPoint, m_socket, SendTimeout, ReceiveTimeout, maxRetries);



        /// <summary>
        /// Gets the rules of the server
        /// </summary>
        /// <returns>RuleResponse containing all rules as a Dictionary</returns>
        /// <exception cref="SourceQueryException"></exception>
        public RuleResponse GetRules(int maxRetries = 10) => GetRulesAsync().GetAwaiter().GetResult();


        /// <summary>
        /// Gets the rules of the server
        /// </summary>
        /// <returns>RuleResponse containing all rules as a Dictionary</returns>
        /// <exception cref="SourceQueryException"></exception>
        public  Task<RuleResponse> GetRulesAsync(int maxRetries = 10) =>
            QueryHelper.GetRulesAsync(m_endPoint, m_socket, SendTimeout, ReceiveTimeout, maxRetries);

        public void Dispose()
        {
            m_socket?.Dispose();
        }
    }
}

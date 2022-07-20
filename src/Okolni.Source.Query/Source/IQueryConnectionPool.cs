using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Okolni.Source.Query.Responses;

namespace Okolni.Source.Query.Source
{
    public interface IQueryConnectionPool : IDisposable
    {

        /// <summary>
        /// Timeout for sending requests (ms). Default is 1000ms * Keep in mind total execution time is timeout*retries*4 (1 send auth, 1 receive auth, 1 send challenge, 1 receive result) *
        /// </summary>
        int SendTimeout { get; set; }
        /// <summary>
        /// Timeout for response from the server (ms). Default is 1000ms * Keep in mind total execution time is timeout*retries*4 (1 send auth, 1 receive auth, 1 send challenge, 1 receive result) *
        /// </summary>
        int ReceiveTimeout { get; set; }





        /// <summary>
        /// Gets the A2S_INFO_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        InfoResponse GetInfo(IPEndPoint endpoint, int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_PLAYERS_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        PlayerResponse GetPlayers(IPEndPoint endpoint, int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_RULES_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        RuleResponse GetRules(IPEndPoint endpoint, int maxRetries = 10);



        /// <summary>
        /// Gets the A2S_INFO_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        Task<InfoResponse> GetInfoAsync(IPEndPoint endpoint, int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_PLAYERS_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        Task<PlayerResponse> GetPlayersAsync(IPEndPoint endpoint, int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_RULES_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        Task<RuleResponse> GetRulesAsync(IPEndPoint endpoint, int maxRetries = 10);
    }
}

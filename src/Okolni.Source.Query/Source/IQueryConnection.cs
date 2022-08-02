using Okolni.Source.Query.Responses;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Okolni.Source.Query
{
    public interface IQueryConnection
    {
        /// <summary>
        /// Host, given as IP or Hostname/Domain
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Valid Port (between 0x0000 and 0xFFFF)
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Connects to the given Address with given Host and Port
        /// </summary>
        /// <param name="timeoutMiliSec">Timeout in miliseconds if no response is received</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        void Connect(int timeoutMiliSec);

        /// <summary>
        /// Connects to the given Address with Host and Port
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        void Connect();

        /// <summary>
        /// Disconnects
        /// </summary>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        void Disconnect();

        /// <summary>
        /// Gets the A2S_INFO_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="ArgumentException"></exception>
        InfoResponse GetInfo(int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_PLAYERS_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="ArgumentException"></exception>
        PlayerResponse GetPlayers(int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_RULES_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="ArgumentException"></exception>
        RuleResponse GetRules(int maxRetries = 10);



        /// <summary>
        /// Gets the A2S_INFO_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="ArgumentException"></exception>
        Task<InfoResponse> GetInfoAsync(int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_PLAYERS_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="ArgumentException"></exception>
        Task<PlayerResponse> GetPlayersAsync(int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_RULES_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="ArgumentException"></exception>
        Task<RuleResponse> GetRulesAsync(int maxRetries = 10);
    }
}

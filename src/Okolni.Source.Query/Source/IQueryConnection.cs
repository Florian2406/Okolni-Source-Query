using Okolni.Source.Query.Responses;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Okolni.Source.Query
{
    public interface IQueryConnection: IDisposable
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
        /// Timeout for sending requests (ms). Default is 1000ms * Keep in mind total execution time is timeout*retries*4 (1 send auth, 1 receive auth, 1 send challenge, 1 receive result) *
        /// </summary>
        int SendTimeout { get; set; }
        /// <summary>
        /// Timeout for response from the server (ms). Default is 1000ms * Keep in mind total execution time is timeout*retries*4 (1 send auth, 1 receive auth, 1 send challenge, 1 receive result) *
        /// </summary>
        int ReceiveTimeout { get; set; }









        /// <summary>
        /// Connects to the given Address with given Host and Port
        /// </summary>
        /// <param name="timeoutMiliSec">Timeout in miliseconds if no response is received</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        [Obsolete("UDP is connection-less, Switch to Setup instead")]
        void Connect(int timeoutMiliSec);

        /// <summary>
        /// Connects to the given Address with Host and Port
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        [Obsolete("UDP is connection-less, Switch to Setup instead")]

        void Connect();



        /// <summary>
        /// Sets up a connection the given Address with Host and Port. This doesn't actually connect, just sets up the endpoint/udp socket.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        void Setup();

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
        InfoResponse GetInfo(int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_PLAYERS_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        PlayerResponse GetPlayers(int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_RULES_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        RuleResponse GetRules(int maxRetries = 10);



        /// <summary>
        /// Gets the A2S_INFO_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        Task<InfoResponse> GetInfoAsync(int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_PLAYERS_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        Task<PlayerResponse> GetPlayersAsync(int maxRetries = 10);

        /// <summary>
        /// Gets the A2S_RULES_RESPONSE from the server
        /// </summary>
        /// <param name="maxRetries">How often the get info should be retried if the server responds with a challenge request</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        Task<RuleResponse> GetRulesAsync(int maxRetries = 10);
    }
}

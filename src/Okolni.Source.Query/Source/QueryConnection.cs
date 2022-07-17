using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Okolni.Source.Common;
using Okolni.Source.Common.ByteHelper;
using Okolni.Source.Query.Responses;

namespace Okolni.Source.Query
{
    public class QueryConnection : IQueryConnection, IDisposable
    {
        private UdpClient m_udpClient;       
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

            if (m_udpClient != null || (m_udpClient != null && m_udpClient.Client.Connected))
                return;

            m_udpClient = new UdpClient();
            m_endPoint = new IPEndPoint(IPAddress.Parse(Host), Port);
        }

        /// <inheritdoc />
        public void Connect(int timeoutMiliSec)
        {
            if (string.IsNullOrEmpty(Host))
                throw new ArgumentNullException(nameof(Host), "A Host must be specified");
            if (Port < 0x0 || Port > 0xFFFF)
                throw new ArgumentOutOfRangeException(nameof(Port), "A Valid Port has to be specified (between 0x0000 and 0xFFFF)");

            if (m_udpClient != null || (m_udpClient != null && m_udpClient.Client.Connected))
                return;

            m_udpClient = new UdpClient();
            m_endPoint = new IPEndPoint(IPAddress.Parse(Host), Port);
            m_udpClient.Connect(m_endPoint);
            // Note: The timeout does nothing now because SendTimeout and Receive timeout only affect the sync Recieve/Send methods..
            m_udpClient.Client.SendTimeout = timeoutMiliSec;
            m_udpClient.Client.ReceiveTimeout = timeoutMiliSec;
        }

        /// <inheritdoc />
        public void Connect()
        {
            Connect(5000);
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (m_udpClient != null)
            {
                if (m_udpClient.Client != null && m_udpClient.Client.Connected)
                {
                    m_udpClient.Client.Disconnect(false);
                }
                m_udpClient.Dispose();
            }
        }

        private async Task Request(byte[] requestMessage)
        {
            var newCancellationToken = new CancellationTokenSource();
            newCancellationToken.CancelAfter(SendTimeout);
            try
            {
                ValueTask<int> sendTask;


                sendTask = m_udpClient.SendAsync(new ReadOnlyMemory<byte>(requestMessage), m_endPoint,
                    newCancellationToken.Token);

                var send = await sendTask;

            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Send Request took longer then the specified timeout {SendTimeout}");
            }
        }

        private async Task<byte[]> ReceiveAsync()
        {
            var newCancellationToken = new CancellationTokenSource();
            newCancellationToken.CancelAfter(ReceiveTimeout);
            try
            {
                Memory<byte> buffer = new byte[65527];
                var udpClientRecieve = await m_udpClient.Client.ReceiveAsync(buffer, SocketFlags.None, newCancellationToken.Token);
                var recvPacket = buffer.Slice(0, udpClientRecieve);
                return recvPacket.ToArray();
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Recieve took longer then the specified timeout {ReceiveTimeout}");
            }
        }

        private async Task<byte[]> FetchResponse()
        {
            var response = await ReceiveAsync();
            var byteReader = response.GetByteReader();
            var header = byteReader.GetUInt();
            if (header.Equals(Constants.SimpleResponseHeader))
            {
                return byteReader.GetRemaining();
            }
            else
            {
                return await FetchMultiPacketResponse(byteReader);
            }
        }

        private async Task<byte[]> FetchMultiPacketResponse(IByteReader byteReader)
        {
            var firstResponse = new MultiPacketResponse { Id = byteReader.GetUInt(), Total = byteReader.GetByte(), Number = byteReader.GetByte(), Size = byteReader.GetShort(), Payload = byteReader.GetRemaining() };

            var compressed = (firstResponse.Id & 2147483648) == 2147483648; // Check for compression

            var responses = new List<MultiPacketResponse>(new[] { firstResponse });
            for (int i = 1; i < firstResponse.Total; i++)
            {
                var response = await ReceiveAsync();
                var multiResponseByteReader = response.GetByteReader();
                var header = multiResponseByteReader.GetUInt();
                if (header != Constants.MultiPacketResponseHeader)
                {
                    i--;
                    continue;
                }
                var id = multiResponseByteReader.GetUInt();
                if (id != firstResponse.Id)
                {
                    i--;
                    continue;
                }
                responses.Add(new MultiPacketResponse { Id = id, Total = multiResponseByteReader.GetByte(), Number = multiResponseByteReader.GetByte(), Size = multiResponseByteReader.GetShort(), Payload = multiResponseByteReader.GetRemaining() });
            }

            var assembledPayload = AssembleResponses(responses);
            var assembledPayloadByteReader = assembledPayload.GetByteReader();

            if (compressed)
                throw new NotImplementedException("Compressed responses are not yet implemented");

            var payloadHeader = assembledPayloadByteReader.GetUInt();

            return assembledPayloadByteReader.GetRemaining();
        }

        private byte[] AssembleResponses(IEnumerable<MultiPacketResponse> responses)
        {
            responses = responses.OrderBy(x => x.Number);
            List<byte> assembledPayload = new List<byte>();

            foreach (var response in responses)
            {
                assembledPayload.AddRange(response.Payload);
            }

            return assembledPayload.ToArray();
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
        public async Task<InfoResponse> GetInfoAsync(int maxRetries = 10)
        {
            try
            {
                var requestData = await RequestDataFromServer(Constants.A2S_INFO_REQUEST, maxRetries);

                var byteReader = requestData.reader;
                var header = requestData.header;

                if (header != Constants.A2S_INFO_RESPONSE)
                    throw new ArgumentException("The fetched Response is no A2S_INFO Response.");

                InfoResponse res = new InfoResponse();

                res.Header = header;
                res.Protocol = byteReader.GetByte();
                res.Name = byteReader.GetString();
                res.Map = byteReader.GetString();
                res.Folder = byteReader.GetString();
                res.Game = byteReader.GetString();
                res.ID = byteReader.GetUShort();
                res.Players = byteReader.GetByte();
                res.MaxPlayers = byteReader.GetByte();
                res.Bots = byteReader.GetByte();
                res.ServerType = byteReader.GetByte().ToServerType();
                res.Environment = byteReader.GetByte().ToEnvironment();
                res.Visibility = byteReader.GetByte().ToVisibility();
                res.VAC = byteReader.GetByte() == 0x01;
                //Check for TheShip
                if (res.ID == 2400)
                {
                    res.Mode = byteReader.GetByte().ToTheShipMode();
                    res.Witnesses = byteReader.GetByte();
                    res.Duration = TimeSpan.FromSeconds(byteReader.GetByte());
                }
                res.Version = byteReader.GetString();

                //IF Has EDF Flag 
                if (byteReader.Remaining > 0)
                {
                    res.EDF = byteReader.GetByte();

                    if ((res.EDF & Constants.EDF_PORT) == Constants.EDF_PORT)
                    {
                        res.Port = byteReader.GetUShort();
                    }
                    if ((res.EDF & Constants.EDF_STEAMID) == Constants.EDF_STEAMID)
                    {
                        res.SteamID = byteReader.GetULong();
                    }
                    if ((res.EDF & Constants.EDF_SOURCETV) == Constants.EDF_SOURCETV)
                    {
                        res.SourceTvPort = byteReader.GetUShort();
                        res.SourceTvName = byteReader.GetString();
                    }
                    if ((res.EDF & Constants.EDF_KEYWORDS) == Constants.EDF_KEYWORDS)
                    {
                        res.KeyWords = byteReader.GetString();
                    }
                    if ((res.EDF & Constants.EDF_GAMEID) == Constants.EDF_GAMEID)
                    {
                        res.GameID = byteReader.GetULong();
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw new SourceQueryException("Could not gather Info", ex);
            }
        }


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
        public async Task<PlayerResponse> GetPlayersAsync(int maxRetries = 10)
        {
            try
            {
                var requestData = await RequestDataFromServer(Constants.A2S_PLAYER_CHALLENGE_REQUEST, maxRetries, true);

                var byteReader = requestData.reader;
                var header = requestData.header;
                
                if (!header.Equals(Constants.A2S_PLAYER_RESPONSE))
                    throw new ArgumentException("Response was no player response.");

                PlayerResponse playerResponse = new PlayerResponse() { Header = header, Players = new List<Player>() };
                int playercount = byteReader.GetByte();
                for (int i = 1; i <= playercount; i++)
                {
                    playerResponse.Players.Add(new Player()
                    {
                        Index = byteReader.GetByte(),
                        Name = byteReader.GetString(),
                        Score = byteReader.GetUInt(),
                        Duration = TimeSpan.FromSeconds(byteReader.GetFloat())
                    });
                }

                //IF more bytes == THE SHIP
                if (byteReader.Remaining > 0)
                {
                    playerResponse.IsTheShip = true;
                    for (int i = 0; i < playercount; i++)
                    {
                        playerResponse.Players[i].Deaths = byteReader.GetUInt();
                        playerResponse.Players[i].Money = byteReader.GetUInt();
                    }
                }

                return playerResponse;
            }
            catch (Exception ex)
            {
                throw new SourceQueryException("Could not gather Players", ex);
            }
        }



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
        public async Task<RuleResponse> GetRulesAsync(int maxRetries = 10)
        {
            try
            {
                var requestData = await RequestDataFromServer(Constants.A2S_RULES_CHALLENGE_REQUEST,  maxRetries, true);

                var byteReader = requestData.reader;
                var header = requestData.header;

                if (!header.Equals(Constants.A2S_RULES_RESPONSE))
                    throw new ArgumentException("Response was no rules response.");

                RuleResponse ruleResponse = new RuleResponse() { Header = header, Rules = new Dictionary<string, string>() };
                int rulecount = byteReader.GetShort();
                for (int i = 1; i <= rulecount; i++)
                {
                    ruleResponse.Rules.Add(byteReader.GetString(), byteReader.GetString());
                }

                return ruleResponse;
            }
            catch (Exception ex)
            {
                throw new SourceQueryException("Could not gather Rules", ex);
            }
        }
        // This could do with some clean up
        public async Task<(IByteReader reader, byte header)> RequestDataFromServer(byte[] request, int maxRetries, bool replaceLastBytesInRequest = false)
        {
            int retries = 0;
            // Always try at least once...
            do
            {
                try
                {
                    await Request(request);
                    var response = await FetchResponse();

                    var byteReader = response.GetByteReader();
                    byte header = byteReader.GetByte();

                    if (header == Constants
                            .CHALLENGE_RESPONSE) // Header response is a challenge response so the challenge must be sent as well
                    {
                        do
                        {
                            var retryRequest = request;
                            // Note for future: Nothing guarantees the A2S_Info Challenge will always be 4 bytes. A2S_Players and A2S_Rules Challenge length is defined in spec/dev docs, but not A2S_Info.
                            var challenge = byteReader.GetBytes(4);
                            if (replaceLastBytesInRequest)
                                retryRequest.InsertArray(retryRequest.Length - 4, challenge);
                            else
                                Helper.AppendToArray(ref retryRequest, challenge);

                            await Request(retryRequest);

                            var retryResponse = await FetchResponse();
                            byteReader = retryResponse.GetByteReader();
                            header = byteReader.GetByte();

                            retries++;
                        } while (header == Constants.CHALLENGE_RESPONSE && retries < maxRetries);

                        if (header == Constants.CHALLENGE_RESPONSE)
                            throw new SourceQueryException($"Retry limit exceeded for the request.  Tried {retries} times, but couldn't get non-challenge packet.");
                    }

                    return (byteReader, header);
                }
                // Any timeout is just another signal to continue
                catch (TimeoutException) { /* Nom */}
                finally
                {
                    // Intellij gets confused, keep in mind above we are returning the byteReader and header if everything else is successful
                    retries++;
                }
            }
            while (retries < maxRetries);
            throw new SourceQueryException($"Retry limit exceeded for the request. Tried {retries} times.");
        }

        public void Dispose()
        {
            m_udpClient?.Dispose();
        }
    }
}

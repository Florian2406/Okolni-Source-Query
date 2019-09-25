using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Okolni.Source.Common;
using Okolni.Source.Common.ByteHelper;
using Okolni.Source.Query.Responses;
using static Okolni.Source.Common.Enums;

namespace Okolni.Source.Query
{
    public class QueryConnection : IQueryConnection
    {
        private UdpClient m_udpClient;
        private IPEndPoint m_endPoint;

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
            if(m_udpClient != null)
            {
                if (m_udpClient.Client != null && m_udpClient.Client.Connected)
                {
                    m_udpClient.Client.Disconnect(false);
                }
                m_udpClient.Dispose();
            }
        }

        private void request(byte[] requestMessage)
        {
            m_udpClient.Send(requestMessage, requestMessage.Length);
        }

        private byte[] fetchResponse()
        {
            byte[] response = m_udpClient.Receive(ref m_endPoint);
            IByteReader byteReader = response.GetByteReader();
            if (byteReader.GetLong().Equals(Constants.SimpleResponseHeader))
            {
                return byteReader.GetRemaining();
            }
            else
            {
                throw new NotImplementedException("Mulitpacket Responses are not yet supported.");
            }
        }

        /// <summary>
        /// Gets the servers general informations
        /// </summary>
        /// <returns>InfoResponse containing all Infos</returns>
        /// <exception cref="SourceQueryException"></exception>
        public InfoResponse GetInfo()
        {
            try
            {
                request(Constants.A2S_INFO_REQUEST);
                var response = fetchResponse();

                var byteReader = response.GetByteReader();
                byte header = byteReader.GetByte();
                if (header != 0x49)
                    throw new ArgumentException("The fetched Response is no A2S_INFO Response.");

                InfoResponse res = new InfoResponse();

                res.Header = header;
                res.Protocol = byteReader.GetByte();
                res.Name = byteReader.GetString();
                res.Map = byteReader.GetString();
                res.Folder = byteReader.GetString();
                res.Game = byteReader.GetString();
                res.ID = byteReader.GetShort();
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

                    if ((res.EDF & 0x80) == 1)
                    {
                        res.Port = byteReader.GetShort();
                    }
                    if ((res.EDF & 0x10) == 1)
                    {
                        res.SteamID = byteReader.GetLong();
                    }
                    if ((res.EDF & 0x40) == 1)
                    {
                        res.SourceTvPort = byteReader.GetShort();
                        res.SourceTvName = byteReader.GetString();
                    }
                    if ((res.EDF & 0x20) == 1)
                    {
                        res.KeyWords = byteReader.GetString();
                    }
                    if ((res.EDF & 0x01) == 1)
                    {
                        res.GameID = byteReader.GetLong();
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
        public PlayerResponse GetPlayers()
        {
            try
            {
                var req = Constants.A2S_PLAYER_CHALLENGE_REQUEST;

                request(req);
                byte[] response = fetchResponse();
                IByteReader byteReader = response.GetByteReader();
                if (!byteReader.GetByte().Equals(0x41))
                    throw new ArgumentException("Response was no challenge response.");
                req[5] = byteReader.GetByte();
                req[6] = byteReader.GetByte();
                req[7] = byteReader.GetByte();
                req[8] = byteReader.GetByte();
                request(req);
                response = fetchResponse();
                byteReader = response.GetByteReader();
                byte header = byteReader.GetByte();
                if (!header.Equals(0x44))
                    throw new ArgumentException("Response was no player response.");

                PlayerResponse playerResponse = new PlayerResponse() { Header = header, Players = new List<Player>() };
                int playercount = byteReader.GetByte();
                for (int i = 1; i <= playercount; i++)
                {
                    playerResponse.Players.Add(new Player()
                    {
                        Index = byteReader.GetByte(),
                        Name = byteReader.GetString(),
                        Score = byteReader.GetLong(),
                        Duration = TimeSpan.FromSeconds(byteReader.GetFloat())
                    });
                }

                //IF more bytes == THE SHIP
                if (byteReader.Remaining > 0)
                {
                    playerResponse.IsTheShip = true;
                    for (int i = 0; i < playercount; i++)
                    {
                        playerResponse.Players[i].Deaths = byteReader.GetLong();
                        playerResponse.Players[i].Money = byteReader.GetLong();
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
        public RuleResponse GetRules()
        {
            try
            {
                var req = Constants.A2S_RULES_CHALLENGE_REQUEST;

                request(req);
                byte[] response = fetchResponse();
                IByteReader byteReader = response.GetByteReader();
                if (!byteReader.GetByte().Equals(0x41))
                    throw new ArgumentException("Response was no challenge response.");
                req[5] = byteReader.GetByte();
                req[6] = byteReader.GetByte();
                req[7] = byteReader.GetByte();
                req[8] = byteReader.GetByte();
                request(req);
                response = fetchResponse();
                byteReader = response.GetByteReader();
                byte header = byteReader.GetByte();
                if (!header.Equals(0x45))
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
    }
}

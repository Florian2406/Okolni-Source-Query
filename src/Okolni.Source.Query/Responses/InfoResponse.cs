using Okolni.Source.Common;
using System;
using System.Collections.Generic;
using System.Text;
using static Okolni.Source.Common.Enums;

namespace Okolni.Source.Query.Responses
{
    public class InfoResponse : IResponse
    {
        /// <summary>
        /// Always equal to 'I' (0x49)
        /// </summary>
        public byte Header { get; set; }

        /// <summary>
        /// Protocol version used by the server.
        /// </summary>
        public byte Protocol { get; set; }

        /// <summary>
        /// Name of the server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Map the server has currently loaded.
        /// </summary>
        public string Map { get; set; }

        /// <summary>
        /// Name of the folder containing the game files.
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// Full name of the game.
        /// </summary>
        public string Game { get; set; }

        /// <summary>
        /// Steam Application ID of game. (Steam Application ID: https://developer.valvesoftware.com/wiki/Steam_Application_IDs)
        /// </summary>
        public ushort ID { get; set; }

        /// <summary>
        /// Number of players on the server.
        /// </summary>
        public byte Players { get; set; }

        /// <summary>
        /// Maximum number of players the server reports it can hold.
        /// </summary>
        public byte MaxPlayers { get; set; }

        /// <summary>
        /// Number of bots on the server.
        /// </summary>
        public byte Bots { get; set; }

        /// <summary>
        /// Indicates the type of server
        /// </summary>
        public ServerType ServerType { get; set; }

        /// <summary>
        /// Indicates the operating system of the server
        /// </summary>
        public Enums.Environment Environment { get; set; }

        /// <summary>
        /// Indicates whether the server requires a password
        /// </summary>
        public Visibility Visibility { get; set; }

        /// <summary>
        /// Specifies whether the server uses VAC:
        /// false for unsecured
        /// true for secured
        /// </summary>
        public bool VAC { get; set; }

        /// <summary>
        /// [ONLY AVAILABLE IF THE SERVER IS RUNNING 'The Ship'] Indicates the game mode
        /// </summary>
        public TheShipMode? Mode { get; set; }

        /// <summary>
        /// [ONLY AVAILABLE IF THE SERVER IS RUNNING 'The Ship'] The number of witnesses necessary to have a player arrested.
        /// </summary>
        public byte? Witnesses { get; set; }

        /// <summary>
        /// [ONLY AVAILABLE IF THE SERVER IS RUNNING 'The Ship'] Time (in seconds) before a player is arrested while being witnessed.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Version of the game installed on the server.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// If present, this specifies which additional data fields will be included.
        /// </summary>
        public byte? EDF { get; set; }

        /// <summary>
        /// if ( EDF & 0x80 ) proves true: The server's game port number.
        /// </summary>
        public ushort? Port { get; set; }

        /// <summary>
        /// if ( EDF & 0x10 ) proves true: Server's SteamID.
        /// </summary>
        public ulong? SteamID { get; set; }

        /// <summary>
        /// if ( EDF & 0x40 ) proves true: Spectator port number for SourceTV.
        /// </summary>
        public ushort? SourceTvPort { get; set; }

        /// <summary>
        /// if ( EDF & 0x40 ) proves true: Name of the spectator server for SourceTV.
        /// </summary>
        public string SourceTvName { get; set; }

        /// <summary>
        /// if ( EDF & 0x20 ) proves true: Tags that describe the game according to the server (for future use.)
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// if ( EDF & 0x01 ) proves true: The server's 64-bit GameID. 
        /// If this is present, a more accurate AppID is present in the low 24 bits. 
        /// The earlier AppID could have been truncated as it was forced into 16-bit storage.
        /// </summary>
        public ulong? GameID { get; set; }

        /// <summary>
        /// If the Server is a The Ship Server
        /// </summary>
        public bool IsTheShip => ID == 2400;

        /// <summary>
        /// If the Info contains a game Port
        /// </summary>
        public bool HasPort => Port != null;

        /// <summary>
        /// If the Info contains a SteamId
        /// </summary>
        public bool HasSteamID => SteamID != null;

        /// <summary>
        /// If the Info contains SourceTv Information
        /// </summary>
        public bool HasSourceTv => SourceTvPort != null && SourceTvName != null;

        /// <summary>
        /// If the Info contains KeyWords
        /// </summary>
        public bool HasKeywords => KeyWords != null;

        /// <summary>
        /// If the Info contains a GameID
        /// </summary>
        public bool HasGameID => GameID != null;

        public override string ToString()
        {
            string response = string.Empty;
            response += $" Header: 0x{Header.ToString("X2")};";
            response += $" Protocol: {Protocol.ToString()};";
            response += $" Name: {Name};";
            response += $" Map: {Map};";
            response += $" Folder: {Folder};";
            response += $" Game: {Game};";
            response += $" ID: {ID};";
            response += $" Players: {Players};";
            response += $" Max. Players: {MaxPlayers};";
            response += $" Bots: {Bots};";
            response += $" Server type: {ServerType};";
            response += $" Environment: {Environment};";
            response += $" Visibility: {Visibility};";
            response += $" VAC: {VAC};";
            if (IsTheShip)
            {
                response += $" Mode: {Mode};";
                response += $" Witnesses: {Witnesses};";
                response += $" Duration: {Duration};";
            }
            response += $" Version: {Version};";
            if (HasGameID || HasPort || HasKeywords || HasSourceTv || HasSteamID)
            {
                if (HasPort)
                    response += $" Port: {Port};";
                if (HasSteamID)
                    response += $" SteamID: {SteamID};";
                if (HasSourceTv)
                {
                    response += $" SourceTvPort: {SourceTvPort};";
                    response += $" SourceTvName: {SourceTvName};";
                }
                if (HasKeywords)
                    response += $" Keywords: {KeyWords};";
                if (HasGameID)
                    response += $" GameID: {GameID};";
            }
            return response;
        }
    }
}

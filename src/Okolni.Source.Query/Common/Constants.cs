using System;
using System.Collections.Generic;
using static Okolni.Source.Common.Enums;

namespace Okolni.Source.Common
{
    public static class Constants
    {
        // CONSTANTS

        /// <summary>
        /// The byte indicator of the challenge response.
        /// </summary>
        public const byte CHALLENGE_RESPONSE = 0x41; //Ignoring C# naming conventions as of the original name defined by valve

        /// <summary>
        /// The byte indicator of the info response.
        /// </summary>
        public const byte A2S_INFO_RESPONSE = 0x49; //Ignoring C# naming conventions as of the original name defined by valve

        /// <summary>
        /// The byte indicator of the player response.
        /// </summary>
        public const byte A2S_PLAYER_RESPONSE = 0x44; //Ignoring C# naming conventions as of the original name defined by valve

        /// <summary>
        /// The byte indicator of the rules response.
        /// </summary>
        public const byte A2S_RULES_RESPONSE = 0x45; //Ignoring C# naming conventions as of the original name defined by valve

        /// <summary>
        /// The Header for a simple response - Always equal to -1 (0xFFFFFFFF). Means it isn't split.
        /// </summary>
        public const int SimpleResponseHeader = -1;

        /// <summary>
        /// The Header for a multi packet response - Always equal to -2 (0xFFFFFFFE). Means the packet is split. 
        /// </summary>
        public const int MultiPacketResponseHeader = -2;

        /// <summary>
        /// The game id for 'The Ship'
        /// </summary>
        public const short TheShipGameId = 2400;

        // STATIC ARRAYS

        /// <summary>
        /// Retrieves information about the server including, but not limited to: its name, the map currently being played, and the number of players.
        /// </summary>
        public static byte[] A2S_INFO_REQUEST = new byte[]
        {
                0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69,
                0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00
        };

        /// <summary>
        /// This query retrieves information about the players currently on the server. It needs an initial step to acquire a challenge number.
        /// </summary>
        public static byte[] A2S_PLAYER_CHALLENGE_REQUEST = new byte[] {
            0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF
        };

        /// <summary>
        /// Returns the server rules, or configuration variables in name/value pairs. This query requires an initial challenge step.
        /// </summary>
        public static byte[] A2S_RULES_CHALLENGE_REQUEST = new byte[] {
            0xFF, 0xFF, 0xFF, 0xFF, 0x56, 0xFF, 0xFF, 0xFF, 0xFF
        };

        /// <summary>
        /// Ping the server to see if it exists, this can be used to calculate the latency to the server.
        /// </summary>
        public static byte[] A2A_PING_REQUEST = new byte[] {
            0xFF, 0xFF, 0xFF, 0xFF, 0x69
        };

        /// <summary>
        /// A2S_PLAYER and A2S_RULES queries both require a challenge number. Formerly, this number could be obtained via an A2S_SERVERQUERY_GETCHALLENGE request.
        /// </summary>
        public static byte[] A2S_SERVERQUERY_GETCHALLENGE_REQUEST = new byte[]
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x57
        };

        /// <summary>
        /// Mapping for byte to 'ServerType' conversion
        /// </summary>
        public static Dictionary<byte, ServerType> ByteServerTypeMapping = new Dictionary<byte, ServerType>() {
            { 0x64, ServerType.Dedicated },
            { 0x6c, ServerType.NonDedicated },
            { 0x70, ServerType.SourceTvRelay },
            { 0x00, ServerType.NonDedicated } //Rag Doll Kung Fu
        };

        /// <summary>
        /// Mapping for byte to 'Environment' conversion
        /// </summary>
        public static Dictionary<byte, Enums.Environment> ByteEnvironmentMapping = new Dictionary<byte, Enums.Environment>()
        {
            { 0x6c, Enums.Environment.Linux },
            { 0x77, Enums.Environment.Windows },
            { 0x6d, Enums.Environment.Mac },
            { 0x6f, Enums.Environment.Mac }
        };

        /// <summary>
        /// Mapping for byte to 'TheShipMode' conversion
        /// </summary>
        public static Dictionary<byte, TheShipMode> ByteTheShipModeMapping = new Dictionary<byte, TheShipMode>()
        {
            { 0x00, TheShipMode.Hunt },
            { 0x01, TheShipMode.Elimination },
            { 0x02, TheShipMode.Duel },
            { 0x03, TheShipMode.Deathmatch },
            { 0x04, TheShipMode.VipTeam },
            { 0x05, TheShipMode.TeamElimination }
        };

        /// <summary>
        /// Mapping for byte to 'Visibility' conversion
        /// </summary>
        public static Dictionary<byte, Visibility> ByteVisibilityMapping = new Dictionary<byte, Visibility>()
        {
            { 0x00, Visibility.Public },
            { 0x01, Visibility.Private }
        };
    }
}

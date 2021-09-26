namespace Okolni.Source.Common
{
    /// <summary>
    /// Enums used in Okolni Source package. Resembles the informations in the responses
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// The engine used
        /// </summary>
        public enum Engine
        {
            Goldsource,
            Source
        }

        /// <summary>
        /// The server type
        /// </summary>
        public enum ServerType
        {
            Dedicated,
            NonDedicated,
            SourceTvRelay
        }

        /// <summary>
        /// The environment/os
        /// </summary>
        public enum Environment
        {
            Linux,
            Windows,
            Mac
        }

        /// <summary>
        /// Mode of the TheShip Server
        /// </summary>
        public enum TheShipMode
        {
            Hunt,
            Elimination,
            Duel,
            Deathmatch,
            VipTeam,
            TeamElimination
        }

        /// <summary>
        /// The visibility of the server
        /// </summary>
        public enum Visibility
        {            Public,
            Private
        }
    }
}

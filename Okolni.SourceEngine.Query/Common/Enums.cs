namespace Okolni.SourceEngine.Common
{
    public static class Enums
    {
        public enum Engine
        {
            Goldsource,
            Source
        }

        public enum ServerType
        {
            Dedicated,
            NonDedicated,
            SourceTvRelay
        }

        public enum Environment
        {
            Linux,
            Windows,
            Mac
        }

        public enum TheShipMode
        {
            Hunt,
            Elimination,
            Duel,
            Deathmatch,
            VipTeam,
            TeamElimination
        }

        public enum Visibility
        {
            Public,
            Private
        }
    }
}

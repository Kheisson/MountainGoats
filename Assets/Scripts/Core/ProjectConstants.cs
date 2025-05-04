namespace Core
{
    public static class ProjectConstants
    {
        public class Global
        {
            public const string PROJECT_NAME = "MgGame";

        }
        
        public class Scenes
        {
            public const string BOOTSTRAPPER = "Bootstrapper";
            public const string MAIN_MENU = "MainMenu";
            public const string GAME = "Game";
            public const string GARBAGE_DEMO = "GarbageDemo";
        }
        
        public class Events
        {
            public const string PLAY_STARTED = "play_started";
            public const string PLAY_ENDED = "play_ended";
            public const string GARBAGE_COLLECTED = "garbage_collected";
            public const string GARBAGE_HOOKED = "garbage_hooked";
        }
    }
}
namespace Hangman.Core
{
    public static class Constants
    {
        /// <summary>
        /// Default location of config file on *NIX systems
        /// </summary>
        public const string LinuxConfigPath = "/etc/config/appsettings.json";

        /// <summary>
        /// Known root configuration sections
        /// </summary>
        public static class ConfigSections
        {
            public const string Rabbit = "rabbit";
            public const string Mongo = "mongo";
        }
    }
}

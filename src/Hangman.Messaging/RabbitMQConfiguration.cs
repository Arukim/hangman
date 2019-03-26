namespace Hangman.Messaging
{
    public class RabbitMQConfiguration
    {
        public string Endpoint { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// Used to support running several instances of App
        /// on same RMQ server. Usefull for having multiple
        /// Environments reusing same RMQ instance.
        /// </summary>
        public string Root { get; set; }
    }
}

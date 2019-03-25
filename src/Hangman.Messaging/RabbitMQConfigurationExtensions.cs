using System;

namespace Hangman.Messaging
{
    public static class RabbitMQConfigurationExtensions
    {
        /// <summary>
        /// Build RMQ endpoint address for known @queue
        /// </summary>
        /// <param name="queue">Name of the queue</param>
        /// <returns>RMQ endpoint</returns>
        public static Uri GetEndpoint(this RabbitMQConfiguration config, string queue)
            => new Uri($"{config.Endpoint}/{config.Root}.{queue}");

        public static string GetQueueName(this RabbitMQConfiguration config, string queue)
           => $"{config.Root}.{queue}";
    }
}

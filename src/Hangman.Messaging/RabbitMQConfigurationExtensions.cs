using System;

namespace Hangman.Messaging
{
    public static class RabbitMQConfigurationExtensions
    {
        /// <summary>
        /// Build RMQ endpoint address for known @queue with root prefix
        /// </summary>
        /// <param name="queue">Name of the queue</param>
        /// <returns>RMQ endpoint</returns>
        public static Uri GetEndpoint(this RabbitMQConfiguration config, string queue)
            => new Uri($"{config.Endpoint}/{config.Root}.{queue}");

        /// <summary>
        /// Get name of queue with root prefix
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static string GetQueueName(this RabbitMQConfiguration config, string queue)
           => $"{config.Root}.{queue}";
    }
}

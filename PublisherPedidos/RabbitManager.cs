using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitLib
{
    public class RabbitManager
    {
        private readonly IConnection _connection;
        
        public EventingBasicConsumer _eventingBasicConsumer;

        public RabbitManager(RabbitConfig config)
        {
            _connection = CreateConnection(config);
        }

        private IConnection CreateConnection(RabbitConfig config)
        {
            var _connection = new ConnectionFactory
            {
                HostName = config.Host,
                Port = config.Port,
                UserName = config.Username,
                Password = config.Password
            };

            return _connection.CreateConnection();
        }

        public IModel CreateChannel(string queue)
        {
            var channel = _connection.CreateModel();
            channel.QueueDeclare
            (
                queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            return channel;
        }

        public bool NewMessage(IModel channel, string queue, string message)
        {
            try
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish
                    (
                        exchange: "",
                        routingKey: queue,
                        basicProperties: null,
                        body: messageBytes
                    );

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

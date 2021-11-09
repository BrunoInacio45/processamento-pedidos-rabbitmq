using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitLib;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pagamento
{
    public class Worker : BackgroundService
    {
        private readonly RabbitManager _rabbitManager;
        private readonly IModel _channel;

        private readonly RabbitConfig _rabbitConfig;

        public Worker(IConfiguration config)
        {
            _rabbitConfig = config.GetSection("RabbitConfig").Get<RabbitConfig>();
            _rabbitManager = new RabbitManager(_rabbitConfig);
            _channel = _rabbitManager.CreateChannel(_rabbitConfig.Queue);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var content = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(content);
                _channel.BasicAck(eventArgs.DeliveryTag, false);

                Console.WriteLine($"Processando pagamento do pedido");

                ///
                Thread.Sleep(10000); //Simula as validações e processamento do pagamento
                ///

                Console.WriteLine($"Pagamento Aprovado - {message}");

                EnviarParaSeparação(message);
            };

            _channel.BasicConsume(_rabbitConfig.Queue, false, consumer);

            return Task.CompletedTask;
        }

        private void EnviarParaSeparação(string message)
        {
            Console.WriteLine($"Enviando pedido para separação no estoque");
            var channel = _rabbitManager.CreateChannel(_rabbitConfig.NextQueue);
            _rabbitManager.NewMessage(channel, _rabbitConfig.NextQueue, message);
        }
    }
}

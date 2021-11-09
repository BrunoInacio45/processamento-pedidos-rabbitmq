using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitLib;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Separacao
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

                Console.WriteLine($"Iniciando separação de estoque e emissão de nota fiscal");
                ///
                Thread.Sleep(10000); //Simula as validações e processamento
                ///

                Console.WriteLine($"Processo finalizado - {message}");

                EnviarParaTransportadora(message);
            };

            _channel.BasicConsume(_rabbitConfig.Queue, false, consumer);

            return Task.CompletedTask;
        }

        private void EnviarParaTransportadora(string message)
        {
            Console.WriteLine($"Enviando pedido para transportadora");
            var channel = _rabbitManager.CreateChannel(_rabbitConfig.NextQueue);
            _rabbitManager.NewMessage(channel, _rabbitConfig.NextQueue, message);
        }
    }
}

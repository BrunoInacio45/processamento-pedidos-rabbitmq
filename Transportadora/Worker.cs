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

namespace Transportadora
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

                Console.WriteLine($"Iniciando preparação do código de rastreio e envio do produto para ponto de coleta");
                _channel.BasicAck(eventArgs.DeliveryTag, false);

                ///
                Thread.Sleep(10000); //Simula as validações e processamento
                ///

                Console.WriteLine($"Processo finalizado - {message}");

                FinalizaProcessamentoDoPedido(message);
            };

            _channel.BasicConsume(_rabbitConfig.Queue, false, consumer);

            return Task.CompletedTask;
        }

        private void FinalizaProcessamentoDoPedido(string message)
        {
            Console.WriteLine($"{message} enviado para transportadora e fim do ciclo principal do pedido");
        }
    }
}

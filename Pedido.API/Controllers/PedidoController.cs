using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitLib;
using System;

namespace Pedido.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly ILogger<PedidoController> _logger;
        private readonly IConfiguration _configuration;

        public PedidoController(ILogger<PedidoController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult NovoPedido()
        {
            ///// Treco de código abaixo simula a chamada da camada de serviço
            var _rabbitConfig = _configuration.GetSection("RabbitConfig").Get<RabbitConfig>();
            var _rabbitManager = new RabbitManager(_rabbitConfig);
            var channel = _rabbitManager.CreateChannel(_rabbitConfig.Queue);
            var sucesso = _rabbitManager.NewMessage(channel, _rabbitConfig.Queue, $"Pedido {Guid.NewGuid()}");
            /////

            if(sucesso)
                return Created("ok", new { Mensagem = "Enviado para pagamento" });

            return BadRequest(new { Mensagem = "Pedido recusado" });
        }
    }
}

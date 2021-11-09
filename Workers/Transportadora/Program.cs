using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Transportadora;

namespace Separacao
{
    class Transportadora
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (_, services) => {
                        services.AddHostedService<Worker>();
                    }).Build().Run();
        }
    }
}

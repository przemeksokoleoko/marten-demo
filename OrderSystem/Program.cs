using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OrderSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMarten(hostContext.Configuration.GetConnectionString("OrderSystemConnectionString"));
                    services.AddHostedService<Worker>();

                })
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("appsettings.json");
                });

        private class Worker : BackgroundService
        {
            private IDocumentStore _documentStore;
            public Worker(IDocumentStore documentStore)
            {
                _documentStore = documentStore;

                // clean it off
                _documentStore.Advanced.Clean.CompletelyRemoveAll();
            }
            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                var order = new Order
                {
                    Priority = Priority.High,
                    CustomerId = "STEV001",
                    Details = new List<OrderDetail>
                    {
                        new OrderDetail {PartNumber = "10XFX", Number = 5},
                        new OrderDetail {PartNumber = "20XFX", Number = 10}
                    }
                };

                using (var session = _documentStore.LightweightSession())
                {
                    session.Store(order);
                    await session.SaveChangesAsync();
                    // order.id gets assigned with new unique id
                }

                using (var session = _documentStore.LightweightSession())
                {
                    var savedOrder = await session.LoadAsync<Order>(order.Id);
                    System.Console.WriteLine($"{savedOrder.Id} {savedOrder.CustomerId} {savedOrder.Details.Count}");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

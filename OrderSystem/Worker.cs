using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Linq;
using Microsoft.Extensions.Hosting;

namespace OrderSystem
{
    public class Worker : BackgroundService
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
            var orders = Order.GenerateRandomData(10);

            using (var session = _documentStore.LightweightSession())
            {
                session.Store(orders);
                await session.SaveChangesAsync();
                // order.id gets assigned with new unique id
            }

            using (var session = _documentStore.LightweightSession())
            {
                var savedOrder = await session.LoadAsync<Order>(orders[0].Id);
                System.Console.WriteLine($"{savedOrder.Id} {savedOrder.CustomerId} {savedOrder.Details.Count}");
                foreach (var od in orders[0].Details)
                {
                    System.Console.WriteLine($"{od.PartNumber} {od.Number}");
                }
            }

            using (var session = _documentStore.QuerySession())
            {
                var orderCount = await session.QueryAsync(new GetCount());
                System.Console.WriteLine($"#orders in db is {orderCount}");
            }

            await Task.Delay(1000, stoppingToken);
        }

        private class GetCount : ICompiledQuery<Order, int>
        {
            public Expression<Func<IQueryable<Order>, int>> QueryIs()
            {
                return q => q.Count();
            }
        }
    }
}

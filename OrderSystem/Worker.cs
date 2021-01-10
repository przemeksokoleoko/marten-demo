using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Linq;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

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
                    System.Console.WriteLine($"{od}");
                }
            }

            using (var session = _documentStore.QuerySession())
            {
                var orderCount = await session.QueryAsync(new GetCount());
                System.Console.WriteLine($"#orders in db is {orderCount}");
            }

            // event store demo for order system
            var orderStarted = new OrderStarted { Id = Guid.NewGuid(), Name = "Order 1 (events demo)" };

            var addedDetails = new List<OrderDetail>();
            addedDetails.Add(new OrderDetail { PartNumber = "XX123", Number = 10 });
            addedDetails.Add(new OrderDetail { PartNumber = "YY231", Number = 20 });
            addedDetails.Add(new OrderDetail { PartNumber = "ZZ312", Number = 30 });
            var addDetails = new OrderDetailsAdded { OrderDetails = addedDetails, Id = orderStarted.Id };

            var removedDetails = new List<OrderDetail>();
            removedDetails.Add(addedDetails.ElementAt(0));
            var removeDetails = new OrderDetailsRemoved { OrderDetails = removedDetails, Id = orderStarted.Id };

            using (var session = _documentStore.LightweightSession())
            {
                session.Events.StartStream(orderStarted.Id, orderStarted, addDetails, removeDetails);
                session.SaveChanges();
            }

            var newDetails = new List<OrderDetail>();
            newDetails.Add(new OrderDetail { PartNumber = "AA111", Number = 99 });

            var addNewDetails = new OrderDetailsAdded { OrderDetails = newDetails, Id = orderStarted.Id };

            var submitOrder = new OrderSubmitted() { Id = orderStarted.Id, Name = orderStarted.Name };

            using (var session = _documentStore.LightweightSession())
            {
                session.Events.Append(orderStarted.Id, addNewDetails, submitOrder);
                session.SaveChanges();
            }

            using (var session = _documentStore.LightweightSession())
            {
                var orderSummary = session.Events.AggregateStream<OrderSummary>(orderStarted.Id);

                System.Console.WriteLine(JsonConvert.SerializeObject(orderSummary));
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

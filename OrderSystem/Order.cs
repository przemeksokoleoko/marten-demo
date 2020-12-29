using System;
using System.Collections.Generic;
using Bogus;

namespace OrderSystem
{
    public class Order
    {
        public Guid Id { get; set; }

        public Priority Priority { get; set; }
        public string CustomerId { get; set; }
        public IList<OrderDetail> Details { get; set; } = new List<OrderDetail>();

        public static Order[] GenerateRandomData(int count)
        {
            Randomizer.Seed = new Random(65748392);

            var datailsGenerator = new Faker<OrderDetail>()
                .RuleFor(od => od.Number, f => f.Random.Int(0, 100))
                .RuleFor(od => od.PartNumber, f => f.Random.Replace("##???"));

            var orders = new Faker<Order>()
                .RuleFor(o => o.Priority, f => f.PickRandom<Priority>())
                .RuleFor(o => o.CustomerId, f => f.Person.UserName)
                .RuleFor(o => o.Details, f => datailsGenerator.Generate(f.Random.Int(1, 10))
                )
                .Generate(count);

            return orders.ToArray();
        }
    }

    public class OrderDetail
    {
        public string PartNumber { get; set; }
        public int Number { get; set; }
    }

    public enum Priority
    {
        Low,
        High
    }
}
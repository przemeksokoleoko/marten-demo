using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderSystem
{
    public class OrderStarted
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public override string ToString() =>
            $"Order {Name} started";
    }

    public class OrderSubmitted
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public override string ToString() =>
            $"Order {Name} submitted";
    }

    public class OrderDetailsAdded
    {
        public Guid Id { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public override string ToString() =>
            $"Order details {String.Join(" ", OrderDetails)} added to order {Id}";
    }

    public class OrderDetailsRemoved
    {
        public Guid Id { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public override string ToString() =>
            $"Order details {String.Join(" ", OrderDetails)} removed from order {Id}";
    }
}
using System;
using System.Collections.Generic;
using System.Linq;


namespace OrderSystem
{
    public class OrderSummary
    {
        public List<OrderDetail> Details { get; set; } = new List<OrderDetail>();
        public string Name { get; set; }
        public Guid Id { get; set; }

        public void Apply(OrderStarted started)
        {
            Name = started.Name;
            Id = started.Id;
        }
        public void Apply(OrderDetailsAdded added) => 
            Details.AddRange(added.OrderDetails);

        public void Apply(OrderDetailsRemoved removed) =>
            Details.RemoveAll(x => removed.OrderDetails.Contains(x));


    }
}
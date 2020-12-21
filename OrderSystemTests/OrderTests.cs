using System;
using OrderSystem;
using Xunit;

namespace OrderSystemTests.OrderTests
{
    public class OrderTests
    {
        [Fact]
        public void CanCreateAnOrder()
        {
            var order = new Order();
        }
    }

}
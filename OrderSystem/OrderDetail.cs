using System;

namespace OrderSystem
{
    public class OrderDetail : IEquatable<OrderDetail>
    {
        public string PartNumber { get; set; }
        public int Number { get; set; }

        public bool Equals(OrderDetail other) =>
            other is null ? false : (PartNumber.Equals(other.PartNumber) && Number.Equals(other.Number));
        
        public override string ToString() => $"({PartNumber}, {Number})";
    }
}
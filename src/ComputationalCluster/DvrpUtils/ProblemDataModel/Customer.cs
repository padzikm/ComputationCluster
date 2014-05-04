using System;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class Customer : IComparable<Customer>
    {
        public int CustomerId { get; set; }
        public int Duration { get; set; }
        public Point Location { get; set; }
        public int Demand { get; set; }
        public int TimeAvailable { get; set; }

        public int CompareTo(Customer other)
        {
            return CustomerId == other.CustomerId ? 1 : 0;
        }
    }
}

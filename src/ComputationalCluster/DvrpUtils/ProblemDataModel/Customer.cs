using System;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class Customer
    {
        public int CustomerId { get; set; }
        public int Duration { get; set; }
        public Point Location { get; set; }
        public int Demand { get; set; }
        public int TimeAvailable { get; set; }
    }
}

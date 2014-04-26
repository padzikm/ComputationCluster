using System;
using System.Drawing;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class Customer
    {
        public int CustomerId { get; set; }
        public int StartDate { get; set; }
        public int Duration { get; set; }
        public Point Location { get; set; }
    }
}

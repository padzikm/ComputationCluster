using System;
using System.Drawing;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class Customer
    {
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Duration { get; set; }
        public Point Location { get; set; }
    }
}

using System;
using System.Drawing;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class Depot
    {
        public int DepotId { get; set; }
        public DateTime MaxTime { get; set; }
        public DateTime MinTime { get; set; }
        public Point Location { get; set; }

    }
}

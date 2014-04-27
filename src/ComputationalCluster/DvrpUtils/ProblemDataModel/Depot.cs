using System;
using System.Drawing;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class Depot
    {
        public int DepotId { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public Point Location { get; set; }

    }
}

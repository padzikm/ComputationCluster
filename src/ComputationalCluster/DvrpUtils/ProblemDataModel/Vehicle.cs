using System;
using System.Drawing;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public float Capacity { get; set; }
        public float MaxCapacity { get; set; }
        public float Speed { get; set; }
        public Point Location { get; set; }

    }
}

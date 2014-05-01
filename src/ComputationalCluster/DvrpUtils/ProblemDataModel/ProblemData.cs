using System;
using System.Collections;
using System.Collections.Generic;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class ProblemData
    {
        public int VehicleID { get; set; }

        [NonSerialized]
        public string Name { get; set; }

        [NonSerialized]
        public IEnumerable<Customer> Customers { get; set; }

        [NonSerialized]
        public IEnumerable<Vehicle> Vehicles { get; set; }

        [NonSerialized]
        public IEnumerable<Depot> Depots { get; set; }

        // 0 - depot, 1-m - customer
        public IDictionary<int, Point> Path { get; set; }
    }
}

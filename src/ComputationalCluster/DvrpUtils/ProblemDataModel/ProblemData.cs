using System;
using System.Collections;
using System.Collections.Generic;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class ProblemData // ~1000 bytes
    {
        [NonSerialized]
        private IDictionary<int, Point> path;

        public string Name { get; set; } // 20

        public int Capacity { get; set; } // 4

        public int VehiclesCount { get; set; } // 4

        public IEnumerable<Customer> Customers { get; set; } // 32 * ilosc ~ 400

        public IEnumerable<Depot> Depots { get; set; } // 28 * ilosc ~ 300

        public List<int> Partitions { get; set; } // 4 * ilosc ~ 50

        public IDictionary<int, Point> Path
        {
            get { return path; }
            set { path = value; }
        }

        internal ProblemData Clone()
        {
            ProblemData problem = new ProblemData {Capacity = this.Capacity, Customers = this.Customers,
                Depots = this.Depots, Name = this.Name, Partitions = this.Partitions, Path = this.Path, VehiclesCount = this.VehiclesCount};
            return problem;
        }
    }
}

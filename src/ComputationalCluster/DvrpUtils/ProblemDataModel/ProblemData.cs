using System;
using System.Collections;
using System.Collections.Generic;

namespace DvrpUtils.ProblemDataModel
{
    /// <summary>
    /// Opis jednego problemu
    /// </summary>
    [Serializable]
    public class ProblemData
    {
        public string Name { get; set; } 

        public int Capacity { get; set; } 

        public int VehiclesCount { get; set; }

        public IEnumerable<Customer> Customers { get; set; } 

        public IEnumerable<Depot> Depots { get; set; }

        public int PartitionCount { get; set; } 

        internal ProblemData Clone()
        {
            ProblemData problem = new ProblemData {Capacity = this.Capacity, Customers = this.Customers,
                Depots = this.Depots, Name = this.Name, VehiclesCount = this.VehiclesCount};
            return problem;
        }
    }
}

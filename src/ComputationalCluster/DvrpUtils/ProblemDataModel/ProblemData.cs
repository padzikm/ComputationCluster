using System;
using System.Collections;
using System.Collections.Generic;
using ASD.Graph;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class ProblemData
    {
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public IEnumerable<Depot> Depots { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
        public IGraph Graph { get; set; }
    }
}

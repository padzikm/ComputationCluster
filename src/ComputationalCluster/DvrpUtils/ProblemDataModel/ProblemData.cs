using System;
using System.Collections;
using System.Collections.Generic;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class ProblemData
    {
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public IEnumerable<Depot> Depots { get; set; }
        public IEnumerable<Customer> Customers { get; set; }

    }
}

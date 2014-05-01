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
        private int capacity;
        [NonSerialized]
        private string name;
        [NonSerialized]
        private IEnumerable<Customer> customers;
        [NonSerialized]
        private IEnumerable<Vehicle> vehicles;
        [NonSerialized]
        private IEnumerable<Depot> depots;

        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }

        public IEnumerable<Customer> Customers
        {
            get { return customers; }
            set { customers = value; }
        }

        public IEnumerable<Vehicle> Vehicles
        {
            get { return vehicles; }
            set { vehicles = value; }
        }

        public IEnumerable<Depot> Depots
        {
            get { return depots; }
            set { depots = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        // 0 - depot, 1-m - customer
        public IDictionary<int, Point> Path { get; set; }
    }
}

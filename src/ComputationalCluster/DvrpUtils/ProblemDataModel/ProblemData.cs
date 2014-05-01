using System;
using System.Collections;
using System.Collections.Generic;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class ProblemData
    {
        //identyfikacja dla numeru wyznaczonej trasy: zakladam za kazdy partial solution jest dla JEDNEGO pojazdu
        //jest to istotne, poniewaz nie wszystkie grupy tak maja
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
        //_________________________________________________________________________

        //zawiera lokacje depotu Locations[0] - depot oraz lokacje customerow - numery od 1 do m adekwatne do wartosci w 'Path'
        public IEnumerable<Point> Locations { get; set; }

        //zawiera ścieżkę do TSP dla solve, np.: 0->3->8->2->1 co oznacza ze mam odwiedziec w danym partial problem customerow
        // 3, 8, 2, 1, gdzie zajezdnia to 0
        public List<int> Path { get; set; }


    }
}

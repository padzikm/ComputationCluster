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

        //__________________czy aby na pewno 4 ponizsze za potrzebne?______________
        public string Name { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public IEnumerable<Depot> Depots { get; set; }
        //_________________________________________________________________________

        //zawiera lokacje depotu Locations[0] - depot oraz lokacje customerow - numery od 1 do m adekwatne do wartosci w 'Path'
        public IEnumerable<Point> Locations { get; set; }

        //zawiera ścieżkę do TSP dla solve, np.: 0->3->8->2->1 co oznacza ze mam odwiedziec w danym partial problem customerow
        // 3, 8, 2, 1, gdzie zajezdnia to 0
        public List<int> Path { get; set; }
    }
}

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

        /*lista ścieżek - słowników o kluczach jako numery customerow (przy czym 0 - depot), a wartosc to lokacja odpowiadajaca danemu numerowi, przykladowy Paths:
         Path[0][0] = (0,0)
         Path[0][3] = (1,2)
         Path[0][7] = (3,4)
         Path[0][4] = (5,6)
         Path[0][18] = (7,8)
         Path[0][6] = (9,10)
         
         Co oznacza ze mam znalezc najkrotsza sciezke dla 0->3->7->4->18->6->18->0. Po tsp przykladowe rozwiazanie: 0->18->6->3->18->7->4->0
         */
        public IEnumerable<IDictionary<int, Point>> Paths { get; set; }

    }
}

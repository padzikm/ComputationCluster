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
        /// <summary>
        /// Słownik tworzony w metodzie Solve, gdzie klucze to numery Customer'ów w danej ścieżce, a wartości to odpowiadające im lokacje. Używane w metodzie klasy Algorithms o nazwie Run().
        /// </summary>
        [NonSerialized]
        private IDictionary<int, Point> path;

        public string Name { get; set; } 

        public int Capacity { get; set; } 

        public int VehiclesCount { get; set; }

        public IEnumerable<Customer> Customers { get; set; } 

        public IEnumerable<Depot> Depots { get; set; }

        public List<int> Partitions { get; set; } 

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

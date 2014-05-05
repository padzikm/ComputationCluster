using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DvrpUtils.ProblemDataModel;
using UCCTaskSolver;
using System.Threading;
using System.Diagnostics;
using System.Text;

namespace DvrpUtils
{
    public class DVRPTaskSolver : TaskSolver
    {
        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override event UnhandledExceptionEventHandler ErrorOccured;

        public override event TaskSolver.ComputationsFinishedEventHandler ProblemDividingFinished;

        public override event TaskSolver.ComputationsFinishedEventHandler ProblemSolvingFinished;

        public override event TaskSolver.ComputationsFinishedEventHandler SolutionsMergingFinished;

        // Parametr służący do anulowania (wyzerowania czasu) Customerów ze zbyt dużym TimaAvailable
        private static readonly double cutOff = 0.5;

        // Parser czytający plik .vrp i zwracający klasę modelu ProblemData
        private DVRPParser Parser;

        private List<List<Customer>> allCombinations;

        public DVRPTaskSolver(byte[] problemData) : base(problemData) 
        { 
            Parser = new DVRPParser();
            State = TaskSolverState.Idle;
            allCombinations = new List<List<Customer>>();
        }
        
        /// <summary>
        /// Dzielenie problemu polega na wyznaczeniu wszystkich podziałów danego zbioru. Jest to szybkie i nie obciąża Task Managera.
        /// Wysyłane są dane podzbiory, a resztę obliczeń wykonuje Computational Node wraz z metodą Solve.
        /// </summary>
        /// <param name="threadCount"> Ilość dostępnych wątków (node'ów) </param>
        /// <returns> Tablica podproblemów danego problemu. </returns>
        public override byte[][] DivideProblem(int threadCount)
        {
            State = TaskSolverState.Dividing;

            var problem = Parser.Parse(DataSerialization.GetString(_problemData));

            // Podział zbioru na podzbiory
            var customersSet = Partitioning.Partition(problem.Customers.ToList());
            var serializedProblems = new List<byte[]>();
            
            foreach (var partition in customersSet)
            {
                var newProblem = problem.Clone();
                newProblem.Partitions = partition;
                serializedProblems.Add(DataSerialization.BinarySerializeObject(newProblem));
            }
            
            if (ProblemDividingFinished != null) ProblemDividingFinished(new EventArgs(), this);

            return serializedProblems.ToArray();
        }    

        /// <summary>
        /// Merge wybiera jeden najlepszy wynik z wszystkich możliwych. Każdy podproblem jest daje jedno pełne rozwiązanie.
        /// </summary>
        /// <param name="solutions"> Tablica wyników zebranych przez TM. </param>
        public override void MergeSolution(byte[][] solutions)
        {
            State = TaskSolverState.Merging;

            List<double> solut = new List<double>();

            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                solut.Add(DataSerialization.BinaryDeserializeObject<double>(solutions[i]));
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("TOTAL_COST: ").Append(solut.Min()).AppendLine();

            Solution = DataSerialization.GetBytes(sb.ToString());

            SolutionsMergingFinished(new EventArgs(), this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partialData"> Dane otrzymane przez Node w ramach jednego podproblemu. </param>
        /// <param name="timeout"> Czas po jakim obliczenia zostaną zakończone błędem przekroczenia czasu. </param>
        /// <returns> Rozwiązanie (najmniejszy koszt drogi) dla pewnej ilości podproblemów. </returns>
        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            State = TaskSolverState.Solving;

            int timeoutMs = 0;

            timeoutMs += 1000 * timeout.Seconds;
            timeoutMs += 1000 * 60 * timeout.Minutes;
            timeoutMs += timeout.Milliseconds;

            var partialProblemData = DataSerialization.BinaryDeserializeObject<ProblemData>(partialData);

            List<ProblemData> ValidatedProblems = new List<ProblemData>();
            List<Point> points = new List<Point>();
            List<double> finalCosts = new List<double>();

            try
            {
                // Zawartość Compute w nowym wątku
                Compute(() =>
                {
                    if (partialProblemData == null) throw new ArgumentNullException("partialProblemData");

                    // Jedna lista Depotów oraz Customerów, użyta do obliczenia tablicy dwuwymiarowej odległości euklidesowych
                    points.AddRange(partialProblemData.Depots.Select(x => x.Location));
                    points.AddRange(partialProblemData.Customers.Select(x => x.Location));

                    // Inicjalizacja instacji klasy Algorithms, służącej do obliczenia najkrótszej ścieżki metodą Neirest Neighbour, a następnie optymalizacji metodą 2-opt. W tym kroku obliczane są także wszystkie możliwe odległości euklidesowe pomiędzy punktami w points
                    Algorithms tsp = new Algorithms(points);

                    List<List<List<Customer>>> outerList;

                    // Metoda generująca w parametrze out prawidłowe zbiory dla danego podziału. Prawidłowe, tzn. że suma wszystkich Customer'ów dla każdego samochodu jest zbiorem wszystkich Cutomer'ów oraz że żaden z nich się nie powtarza.
                    Partitioning.GenerateValidProblems(partialProblemData.Partitions, partialProblemData.Customers, 0, out outerList);
<<<<<<< HEAD

                    // Walidowanie poprawności wyżej wygenerowanych zbiorów ze względu na założenia DVRP
                    if (ValidatePartition(allCombinations, partialProblemData, out ValidatedProblems))
                    {
                        // Dla każdego poprawnego ze względu na założenia DVRP liczymy najkrótszą ścieżkę oraz zapisujemy do listy wszystkich takich ścieżek
                        foreach (var com in ValidatedProblems)
                            finalCosts.Add(tsp.Run(com.Path.Keys.ToList())); 
                    }            
=======
                    foreach (var list in outerList)
                    {
                        if (ValidatePartition(list, partialProblemData, out ValidatedProblems))
                        {
                            foreach (var com in ValidatedProblems)
                            {
                                path = com.Path.Keys.ToList();
                                cost += tsp.Run(ref path);
                            }

                            finalCosts.Add(cost); // TODO: how to handle final cost?
                        }
                    }

                                  
>>>>>>> b1329d88b3e8fec4667e1fc6d58f1eeb76175294
                }, timeoutMs);

                Console.WriteLine("Ilość kosztów dla różnych możliwości: {0}", finalCosts.Count);

                if (ProblemSolvingFinished != null) ProblemSolvingFinished(new EventArgs(), this);

                // Jeśli zostały znalezione poprawne trasy w danym podziale zwracamy najmniejszy koszt ze wszystkich, jeśli nie -1
                return finalCosts.Count == 0 ? DataSerialization.BinarySerializeObject(-1) : DataSerialization.BinarySerializeObject(finalCosts.Min());
            }
            catch (TimeoutException t)
            {
                State = TaskSolverState.Error | TaskSolverState.Idle;
                ErrorOccured(this, new UnhandledExceptionEventArgs(t, true));

                return null;
            }
            catch (ArgumentNullException a)
            {
                Console.WriteLine("partialProblemData may be null: " + a.Message);

                State = TaskSolverState.Error | TaskSolverState.Idle;
                ErrorOccured(this, new UnhandledExceptionEventArgs(a, true));

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Other exception: " + e.Message);

                State = TaskSolverState.Error | TaskSolverState.Idle;
                ErrorOccured(this, new UnhandledExceptionEventArgs(e, true));

                return null;
            }
        }

        /// <summary>
        /// Walidacja poprawności zestawu zbiorów Customerów. Brane są pod uwagę Capacity każdego Customera w ramach przydziału do jednego samochodu. Jeśli Capacity po zsumowaniu wszystkich w danym przypisaniu do samochodu będzie mniejsze od 0 (dla danego samochodu), rozwiązanie jest odrzucane.
        /// </summary>
        /// <param name="customerSet"> Lista kombinacji Customer'ów. </param>
        /// <param name="problem"> Problem wraz ze wspólnymi elementami takimi jak Lista Depot'ów czy Capacity. </param>
        /// <param name="problemDatasList"> Lista poprawnych kals modelu ProblemData. </param>
        /// <returns> True jeśli co najmmniej jeden zbiór z customerSet jest poprawny. False w p.p. </returns>
        private bool ValidatePartition(IEnumerable<List<Customer>> customerSet, ProblemData problem, out List<ProblemData> problemDatasList)
        {
            //TODO more depots
            double cutOffTime = problem.Depots.First().EndTime * cutOff;
            problemDatasList = new List<ProblemData>();
            int vehicleCount = problem.VehiclesCount;
            //each subSet
            foreach (var subSet in customerSet)
            {
                var data = new ProblemData { Depots = problem.Depots, Path = new Dictionary<int, Point>() };
                int capacity = problem.Capacity;

                foreach (var depot in problem.Depots)
                {
                    data.Path.Add(depot.DepotId, depot.Location);
                }
                foreach (var customer in subSet)
                {
                    if (customer.TimeAvailable > cutOffTime)
                        customer.TimeAvailable = 0;

                    capacity += customer.Demand;
                    data.Path.Add(customer.CustomerId, customer.Location);
                }
                //TODO more depots?
                if (capacity > 0 && vehicleCount > 0)
                {
                    problemDatasList.Add(data);
                    vehicleCount--;
                }
                else
                {
                    vehicleCount = -1;
                    break;
                }
                if (vehicleCount < 0) break;
            }
            if (vehicleCount > 0)
                return true;
            problemDatasList.Clear();
            return false;
        }

        /// <summary>
        /// Metoda wykonująca action w nowym wątku oraz zliczająca czas obliczeń. W razie przekroczeniu timeout'u obliczenia są kończone i metoda rzuca wyjątek TimeoutException.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timeout"></param>
        private void Compute(Action action, int timeout)
        {
            Stopwatch sw = Stopwatch.StartNew();

            ManualResetEvent evt = new ManualResetEvent(false);
            AsyncCallback cb = delegate { evt.Set(); };
            IAsyncResult result = action.BeginInvoke(cb, null);
            if (evt.WaitOne(timeout))
            {
                action.EndInvoke(result);
            }
            else
            {
                throw new TimeoutException();
            }

            sw.Stop();

            Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
        }
    }
}

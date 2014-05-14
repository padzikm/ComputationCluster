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
        List<List<int>> partitions;
        ProblemData problem;
        double akt, min;
        Distances distances;

        public DVRPTaskSolver(byte[] problemData) : base(problemData) 
        { 
            Parser = new DVRPParser();
            State = TaskSolverState.Idle;
        }

        #region Additional methods for Solve(...)
        // Rekurencyjna metoda wyznaczania wszystkich podzbiorów k-elementowych ze zbioru n-elementowego, wraz z możliwymi kombinacjami w ramach wyznaczonego podziału zbioru. Jest to liczba Stirlinga II rodzaju. Po wyznaczeniu danego podziału i danej kombinacji dane są wykorzystywane do dalszej walidacji i liczenia ścieżki. Jest to oszczędne pamięciowo, gdyż nie zapamiętujemy danego podziału, tylko najmniejszy wynik z wszystkich wartości wyznaczonych tą metodą.
        void Stirling2(int n, int k)
        {
            if (n < k || k == 0)
                return;

            if (k == 1)
            {
                partitions[k - 1].AddRange(Enumerable.Range(1, n));

                akt = DVRP(partitions);
                if (akt != -1 && akt < min)
                    min = akt;

                for (int i = 1; i <= n; ++i)
                    partitions[k - 1].Remove(i);

                return;
            }
            if (k == n)
            {
                for (int i = 0; i < k; ++i)
                    partitions[i].Add(i + 1);

                akt = DVRP(partitions);
                if (akt != -1 && akt < min)
                    min = akt;

                for (int i = 0; i < k; ++i)
                    partitions[i].Remove(i + 1);

                return;
            }

            partitions[k - 1].Add(n);
            Stirling2(n - 1, k - 1);
            partitions[k - 1].Remove(n);

            for (int i = 0; i < k; ++i)
            {
                partitions[i].Add(n);
                Stirling2(n - 1, k);
                partitions[i].Remove(n);
            }
        }

        // Metoda wywoływana z metody Stirling2. Zawiera zestaw operacji na jednym konkretnym podziale Customerów na k podzbiorów. Zwraca całkowity koszt trasy wszystkich samochodów w danym podziale, lub -1 jeśli dany podział nie spełnia warunków DVRP.
        double DVRP(List<List<int>> list)
        {
            double cost = 0;
            double totalCost = 0;

            foreach (var com in list)
                if (!PreValidateRoute(com))
                    return -1;

            foreach (var com in list)
            {
                if (TSPwithValidation(com, out cost))
                    totalCost += cost;
                else
                    return -1;
            }

            return totalCost;
        }

        // Algorytm wyznacza wszystkie permutacje Customerów w danej trasie. Po wyznaczeniu każdej permutacji dana trasa jest walidowana ze względu na warunki DVRP. Zwraca true, gdy dowolna permutacja trasy przejdzie walidacje. Wtedy koszt najkrótszej z permutacji zawarty jest w parametrze out minCost. Algorytm ma złożonść czasową o(n*n!), ale pamięciową o(n) - alg w miejscu.
        bool TSPwithValidation(List<int> customers, out double minCost)
        {
            double cost = 0;
            minCost = double.MaxValue;

            int n = customers.Count;
            int[] ints = new int[n];
            List<int> permutation;

            int[] positions = new int[n];
            bool[] used = new bool[n];
            bool last;

            for (int i = 0; i < n; i++)
                positions[i] = i;

            do
            {
                for (int i = 0; i < n; i++)
                    ints[i] = customers[positions[i]];
                permutation = new List<int>(ints);

                if (ValidateRoute(permutation, minCost, out cost))
                    minCost = cost;

                last = false;
                int k = n - 2;
                while (k >= 0)
                {
                    if (positions[k] < positions[k + 1])
                    {
                        for (int i = 0; i < n; i++)
                            used[i] = false;
                        for (int i = 0; i < k; i++)
                            used[positions[i]] = true;
                        do positions[k]++; while (used[positions[k]]);
                        used[positions[k]] = true;
                        for (int i = 0; i < n; i++)
                            if (!used[i]) positions[++k] = i;
                        break;
                    }
                    else k--;
                }
                last = (k < 0);

            } while (!last);

            if (minCost == double.MaxValue)
                return false;

            return true;
        }

        // Zapobieganie kosztownego wyznaczania permutacji (w TSP) zbioru (np. 12! dla 12 Customerów), opierając się na całkowitemu Capacity na danej trasie, które nie może być większe niż Capacity jednego samochodu. W większości przypadków wyeliminuje to duże n! permutacje. Zwraca true, jeśli dana trasa może być obsłużona przez jeden samochów ze względu na Demand Customera.
        bool PreValidateRoute(List<int> customers)
        {
            int capacity = problem.Capacity;

            foreach (var cust in customers)
            {
                Customer customer = problem.Customers.First(x => x.CustomerId == cust);
                capacity += customer.Demand;
            }

            return capacity >= 0 ? true : false;
        }

        // Walidacja jednej konkretnej permutacji trasy dla danego samochodu. Sprawdza czy dana trasa może być obsłużona w czasie działania JEDNEGO depotu, jeśli tak zwraca true oraz długość tej trasy w parametrze wyjściowym length.
        bool ValidateRoute(List<int> customers, double minLength, out double length)
        {
            var firstDepot = problem.Depots.First();
            var lastId = firstDepot.DepotId;
            var previousTime = -1;
            double time = firstDepot.StartTime;
            length = 0;

            foreach (var cust in customers)
            {
                Customer customer = problem.Customers.First(x => x.CustomerId == cust);
                var distance = distances.GetDistance(lastId, cust);

                //if (previousTime != -1 && customer.TimeAvailable < previousTime + distance)
                //    return false;

                if (time < customer.TimeAvailable)
                    time = customer.TimeAvailable;

                time += distance;
                length += distance;
                time += customer.Duration;
                lastId = cust;

                if (time > firstDepot.EndTime) return false;
                if (length > minLength) return false;

                previousTime = customer.TimeAvailable;
            }

            var tmplen = distances.GetDistance(lastId, firstDepot.DepotId);
            time += tmplen;
            length += tmplen;
            if (time > firstDepot.EndTime) return false;

            return true;
        }

        // Sprawdzenie wszystkich Customerów pod względem ich czasu pojawienia się. Jeśli dany czas przekracza 0,5 *depot.endtime to zmieniamy go na 0
        void CutOff()
        {
            for (int i = 0; i < problem.Customers.Count(); ++i)
            {
                double cutoff = problem.Depots.First().EndTime * cutOff;
                if (problem.Customers.ElementAt(i).TimeAvailable > cutoff)
                    problem.Customers.ElementAt(i).TimeAvailable = 0;
            }
        }
        #endregion

        /// <summary>
        /// Dzielenie problemu polega na wyznaczeniu liczb podziału, które wynoszą 1,..., n, gdzie n - liczba Customer'ów w danym problemie. Jest to szybkie i nie obciąża Task Managera.
        /// Wysyłane są dane podzbiory, a resztę obliczeń wykonuje Computational Node wraz z metodą Solve.
        /// </summary>
        /// <param name="threadCount"> Ilość dostępnych wątków (node'ów) </param>
        /// <returns> Tablica podproblemów danego problemu. </returns>
        public override byte[][] DivideProblem(int threadCount)
        {
            State = TaskSolverState.Dividing;

            var problem = Parser.Parse(DataSerialization.GetString(_problemData));

            problem.PartitionsCount = new List<int>();

            var serializedProblems = new List<byte[]>();

            int workForNode = problem.Customers.Count() / threadCount;

            // TODO: co jeśli customers.count == 15 a threadcount = 4? wtedy workfornode 3, ale pozostaja niewykorzystane podzbiory
            for (int i = 0; i < threadCount; ++i)
            {
                for (int j = i * workForNode + 1; j < (i + 1) * workForNode + 1; ++j)
                {
                    if (problem.Customers.Any(x=>x.CustomerId == j))
                        problem.PartitionsCount.Add(j);
                }
                serializedProblems.Add(DataSerialization.BinarySerializeObject(problem));
            }
                
            if (ProblemDividingFinished != null) ProblemDividingFinished(new EventArgs(), this);

            return serializedProblems.ToArray();
        }    

        /// <summary>
        /// Merge wybiera jeden najlepszy wynik z wszystkich możliwych. Każdy podproblem daje jedno pełne rozwiązanie.
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
        /// <param name="partialData"> Dane otrzymane przez Node w ramach pewnego zbioru podproblemów. Dany zbiór może
        /// być 1-elementowy</param>
        /// <param name="timeout"> Czas po jakim obliczenia zostaną zakończone błędem przekroczenia czasu. </param>
        /// <returns> Rozwiązanie (najmniejszy koszt drogi) dla pewnej ilości podproblemów. </returns>
        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            State = TaskSolverState.Solving;

            int timeoutMs = 0;

            timeoutMs += 1000 * timeout.Seconds;
            timeoutMs += 1000 * 60 * timeout.Minutes;
            timeoutMs += timeout.Milliseconds;

            try
            {
                // Zawartość Compute w nowym wątku
                Compute(() =>
                {
                    var partialProblemData = DataSerialization.BinaryDeserializeObject<ProblemData>(partialData);

                    if (partialProblemData == null) throw new ArgumentNullException("partialProblemData");

                    problem = partialProblemData;
                    List<int> k = problem.PartitionsCount;
                    akt = 0;
                    min = double.MaxValue;

                    List<Point> points = new List<Point>();

                    CutOff();

                    points.AddRange(problem.Depots.Select(x => x.Location));
                    points.AddRange(problem.Customers.Select(x => x.Location));

                    distances = new Distances(points);

                    for (int i = 0; i < k.Count; ++i)
                    {
                        partitions = new List<List<int>>();
                        for (int j = 0; j < k[i]; ++j)
                            partitions.Add(new List<int>());

                        Stirling2(partialProblemData.Customers.Count(), k[i]);
                        Console.WriteLine("Minimalny koszt trasy dla podziału na {0} podzbiorów: {1}", k, min);
                    }
                    Console.WriteLine("Minimalny koszt dla wszystkich podproblemów w danym node: {0}", min);

                }, timeoutMs);

                if (ProblemSolvingFinished != null) ProblemSolvingFinished(new EventArgs(), this);

                // Jeśli została znaleziona poprawna trasa w danym podziale zwracamy jej koszt, jeśli nie -1
                return min == double.MaxValue ? DataSerialization.BinarySerializeObject(-1) : DataSerialization.BinarySerializeObject(min);
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

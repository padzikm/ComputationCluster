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

        private static readonly double cutOff = 0.5;

        private DVRPParser Parser;

        private List<List<Customer>> allCombinations;

        public DVRPTaskSolver(byte[] problemData) : base(problemData) 
        { 
            Parser = new DVRPParser();
            State = TaskSolverState.Idle;
            allCombinations = new List<List<Customer>>();
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            State = TaskSolverState.Dividing;

            var problem = Parser.Parse(DataSerialization.GetString(_problemData));

            var customersSet = Partitioning.Partition(problem.Customers.ToList());
            var serializedProblems = new List<byte[]>();
            //todo divide
            foreach (var partition in customersSet)
            {
                var newProblem = problem.Clone();
                newProblem.Partitions = partition;
                serializedProblems.Add(DataSerialization.BinarySerializeObject(newProblem));
            }
            
            if (ProblemDividingFinished != null) ProblemDividingFinished(new EventArgs(), this);

            return serializedProblems.ToArray();
        }

        private bool ValidatePartition(IEnumerable<List<Customer>> customerSet, ProblemData problem, out List<ProblemData> problemDatasList )
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

        // TODO: if (ProblemSolvingFinished != null) TO JEST DOBRZE?
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
            List<int> path = null;
            List<double> finalCosts = new List<double>();
            double cost = 0;

            try
            {
                Compute(() =>
                {
                    if (partialProblemData == null) throw new ArgumentNullException("partialProblemData");

                    points.AddRange(partialProblemData.Depots.Select(x => x.Location));
                    points.AddRange(partialProblemData.Customers.Select(x => x.Location));

                    Algorithms tsp = new Algorithms(points);

                    List<List<List<Customer>>> outerList;

                    Partitioning.GenerateValidProblems(partialProblemData.Partitions, partialProblemData.Customers, 0, out outerList);
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

                                  
                }, timeoutMs);

                Console.WriteLine("Ilość kosztów dla różnych możliwości: {0}", finalCosts.Count);

                if (ProblemSolvingFinished != null) ProblemSolvingFinished(new EventArgs(), this);
                return finalCosts.Count == 0 ? DataSerialization.BinarySerializeObject(-1) : DataSerialization.BinarySerializeObject(finalCosts.Min());
            }
            catch (TimeoutException t)
            {
                State = TaskSolverState.Error | TaskSolverState.Idle;
                ErrorOccured(this, new UnhandledExceptionEventArgs(t, true));

                return null;
            }
        }

        private void Recurse<TList>(Customer[] selected, int index, IEnumerable<TList> remaining) where TList : IEnumerable<Customer>
        {
            IEnumerable<Customer> nextList = remaining.FirstOrDefault();
            if (nextList == null)
            {   
                foreach (Customer i in selected)
                {
                    allCombinations[index].Add(i);
                }             
            }
            else
            {
                foreach (Customer i in nextList)
                {
                    selected[index] = i;
                    Recurse(selected, index + 1, remaining.Skip(1));
                }
            }
        }

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

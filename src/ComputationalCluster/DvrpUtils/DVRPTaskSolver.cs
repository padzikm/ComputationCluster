using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DvrpUtils.ProblemDataModel;
using UCCTaskSolver;
using System.Threading;

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

        public DVRPTaskSolver(byte[] problemData) : base(problemData) 
        { 
            Parser = new DVRPParser();
            State = TaskSolverState.Idle;
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            State = TaskSolverState.Dividing;

            var problem = Parser.Parse(DataSerialization.GetString(_problemData));

            var customersSet = Partitioning.Partition(problem.Customers.ToList());
            var problemsToSend = new List<ProblemData>();

            //TODO more depots?
            double cutOffTime = problem.Depots.First().EndTime * cutOff;
            //each problem set
            int partitions = 0;
            foreach (var set in customersSet)
            {
                List<ProblemData> datas = new List<ProblemData>();
                int vehicleCount = problem.VehiclesCount;
                //each subSet
                foreach (var subSet in set)
                {
                    var data = new ProblemData{Depots = problem.Depots, Path = new Dictionary<int, Point>()};
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
                        data.VehicleID = vehicleCount;
                        datas.Add(data);
                        vehicleCount--;
                    }
                    else
                    {
                        vehicleCount = -1;
                        break;
                    }
                    if (vehicleCount < 0) break;
                }
                if (vehicleCount >= 0)
                {
                    problemsToSend.AddRange(datas);
                    partitions++;
                }
            }

            var serializedProblems = new List<byte[]>();
            foreach (var problems in problemsToSend)
            {
                serializedProblems.Add(DataSerialization.BinarySerializeObject(problems));
            }
            
            if (ProblemDividingFinished != null) ProblemDividingFinished(new EventArgs(), this);

            return serializedProblems.ToArray();
        }

        // TODO: czy jest na 100% poprawnie? zamiast uzywac += string uzywac StringBuilder
        public override void MergeSolution(byte[][] solutions)
        {
            State = TaskSolverState.Merging;

            List<Route> solut = new List<Route>();
            Dictionary<int, double> vehicles = new Dictionary<int, double>();

            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                solut.Add(DataSerialization.BinaryDeserializeObject<Route>(solutions[i]));
            }

            // czy to dobrze dodaje wszystkie unikalne numery aut z listy rozwiazan?
            //vehicles.Keys = solut.Select(x=>x.RouteID).Distinct();

            foreach(var el in solut)
            {
                if (!vehicles.ContainsKey(el.RouteID))
                    vehicles.Add(el.RouteID, double.MaxValue);
            }

            string finalCostString = "";
            string finalString = "";
            double finalCost = 0;

            foreach (var el in solut)
            {
                if (el.Cost < vehicles[el.RouteID])
                    vehicles[el.RouteID] = el.Cost;
            }

            foreach (var el in vehicles)
            {
                finalCost += el.Value;
            }

            finalCostString = String.Format("TOTAL_COST: {0}\n", finalCost);

            //for (int i = 0; i < solut.Count; i++)
            //{
            //    finalString += "ROUTE: ";
            //    foreach(var e in solut[i].Locations)
            //    {
            //        finalString += e.ToString() + " ";
            //    }
            //    finalString += "\n";
            //}
            //finalString += finalCostString;

            //wynikowy string:
            //ROUTE: 1 2 3 55
            //ROUTE: 2 3 4 66
            //TOTAL_COST: 777
            //ostatnia liczba w ROUTE to koszt dla danej drogi

            Solution = DataSerialization.GetBytes(finalString);

            if (finalCost != 0) SolutionsMergingFinished(new EventArgs(), this);
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
 
            Dictionary<int, Point> dictionaryPath = partialProblemData.Path as Dictionary<int, Point>;
            if (dictionaryPath == null) throw new ArgumentNullException("partialData");
            var path = partialProblemData.Path.Keys.ToList();

            try
            {
                double minCost = 0;
   
                Compute(() =>
                    {
                        Algorithms tsp = new Algorithms(dictionaryPath.Values.ToList(), timeoutMs); 
                        minCost = tsp.Run(ref path);                  
                    }, timeoutMs);

                Route route = new Route { RouteID = partialProblemData.VehicleID, Cost = minCost, Locations = path };

                Console.WriteLine(minCost);

                if (ProblemSolvingFinished != null) ProblemSolvingFinished(new EventArgs(), this);

                return DataSerialization.BinarySerializeObject(route);
            }
            catch(TimeoutException t)
            {
                State = TaskSolverState.Error | TaskSolverState.Idle;
                ErrorOccured(this, new UnhandledExceptionEventArgs(t, true));

                return null;
            }
        }

        private void Compute(Action action, int timeout)
        {
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
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DvrpUtils.ProblemDataModel;
using UCCTaskSolver;
using System.Timers;

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

        private Timer timer;

        public DVRPTaskSolver(byte[] problemData) : base(problemData) 
        { 
            Parser = new DVRPParser();
            timer = new Timer(1000);
            State = TaskSolverState.Idle;
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            State = TaskSolverState.Dividing;

            var problem = Parser.Parse(DataSerialization.GetString(_problemData));

            var customersSet = Partitioning.GetAllPartitions(problem.Customers.ToArray()).ToList();
            var problemsToSend = new List<ProblemData>();

            //TODO more depots?
            double cutOffTime = problem.Depots.First().EndTime * cutOff;
            //each problem set
            foreach (var set in customersSet)
            {
                List<ProblemData> datas = new List<ProblemData>();
                int vehicleCount = problem.VehiclesCount;
                //each subSet
                foreach (var subSet in set)
                {
                    var data = new ProblemData{Depots = problem.Depots, Path = new Dictionary<int, Point>()};
                    var locations = new List<Point>();
                    var path = new List<int>();
                    int capacity = problem.Capacity;

                    foreach (var depot in problem.Depots)
                    {
                        locations.Add(depot.Location);
                        path.Add(depot.DepotId);
                    }
                    foreach (var customer in subSet)
                    {
                        if (customer.TimeAvailable > cutOffTime)
                            customer.TimeAvailable = 0;

                        capacity += customer.Demand;
                        problem.Path.Add(customer.CustomerId, customer.Location);
                    }
                    //TODO more depots?
                    if (capacity > 0 && vehicleCount > 0)
                    {
                        data.VehicleID = vehicleCount;
                        datas.Add(data);
                        vehicleCount--;
                    }
                    if (vehicleCount < 0) break;
                }
                if (vehicleCount >= 0)
                    problemsToSend.Concat(datas);
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
            string finalCostString = "";
            string finalString = "";
            double finalCost = 0;

            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                solut.Add(DataSerialization.BinaryDeserializeObject<Route>(solutions[i]));
            }

            foreach (var el in solut)
            {
                finalCost += el.Cost;
            }

            finalCostString = String.Format("TOTAL_COST: {0}\n", finalCost);

            for (int i = 0; i < solut.Count; i++)
            {
                finalString += "ROUTE: ";
                foreach(var e in solut[i].Locations)
                {
                    finalString += e.ToString() + " ";
                }
                finalString += "\n";
            }
            finalString += finalCostString;

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

            double timeoutMs = 0;
            timeoutMs += timeout.Milliseconds;
            timeoutMs += 1000 * timeout.Seconds;
            timeoutMs += 1000 * 60 * timeout.Minutes;

            var partialProblemData = DataSerialization.BinaryDeserializeObject<ProblemData>(partialData);
 
            Dictionary<int, Point> dictionaryPath = partialProblemData.Path as Dictionary<int, Point>;
            if (dictionaryPath == null) throw new ArgumentNullException("partialData");
            var path = partialProblemData.Path.Keys.ToList(); 

            Algorithms tsp = new Algorithms(dictionaryPath.Values.ToList(), timeoutMs);

            try
            {
                double minCost = tsp.Run(ref path);
                Route route = new Route {RouteID = partialProblemData.VehicleID, Cost = minCost, Locations = path};

                Console.WriteLine(minCost);

                if (ProblemSolvingFinished != null) ProblemSolvingFinished(new EventArgs(), this);

                return DataSerialization.BinarySerializeObject(route);
            }
            catch(TimeoutException t)
            {
                State = TaskSolverState.Solving;
                ErrorOccured(this, new UnhandledExceptionEventArgs(t, true));

                return null;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DvrpUtils.ProblemDataModel;
using UCCTaskSolver;

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


        public DVRPTaskSolver(byte[] problemData) : base(problemData)
        {
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            DVRPParser parser = new DVRPParser();
            var problem = parser.Parse(DataSerialization.GetString(_problemData));

            var customersSet = Partitioning.GetAllPartitions(problem.Customers.ToArray()).ToList();
            var problemsToSend = new List<List<ProblemData>>();

            //TODO more depots?
            double cutOffTime = problem.Depots.First().EndTime * cutOff;
            //each problem
            for (int i = 0; i < customersSet.Count; i++)
            {
                List<ProblemData> datas = new List<ProblemData>();
                int vehicleCount = problem.Vehicles.Count();
                //each set
                for (int j = 0; j < customersSet[i].Length; j++)
                {
                    var data = new ProblemData{Depots = problem.Depots};
                    var locations = new List<Point>();
                    var path = new List<int>();
                    int capacity = problem.Capacity;

                    foreach (var depot in problem.Depots)
                    {
                        locations.Add(depot.Location);
                        path.Add(depot.DepotId);
                    }
                    for (int k = 0; k < customersSet[i][j].Length; k++)
                    {
                        var customer = customersSet[i][j][k];
                        if (customer.TimeAvailable > cutOffTime)
                            customer.TimeAvailable = 0;

                        capacity += customer.Size;
                        locations.Add(customer.Location);
                        path.Add(customer.CustomerId);
                    }
                    //TODO more depots?
                    if (capacity > 0 && vehicleCount > 0)
                    {
                        data.Locations = locations;
                        data.Path = path;
                        data.VehicleID = vehicleCount;
                        datas.Add(data);
                        vehicleCount--;
                    }
                    if (vehicleCount < 0) break;
                }
                if (vehicleCount >= 0)
                    problemsToSend.Add(datas);
            }

            var serializedProblems = new List<byte[]>();
            foreach (var problems in problemsToSend)
            {
                serializedProblems.AddRange(problems.Select(DataSerialization.BinarySerializeObject));
            }
            
            if (ProblemDividingFinished != null) ProblemDividingFinished(new EventArgs(), this);
            return serializedProblems.ToArray();

        }

        public override void MergeSolution(byte[][] solutions)
        {
            List<string> solut = new List<string>();
            string final_string = "";
            int final_cost = 0;

            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                solut.Add(DataSerialization.GetString(solutions[i]));

            }

            foreach (var el in solut)
            {
                var tmpR = el.Split(' ');
                final_cost += Convert.ToInt16(tmpR[tmpR.Length - 1]);

            }

            solut.Add(String.Format("TOTAL_COST: {0}", final_cost));

            for (int i = 0; i < solut.Count; i++)
            {
                if (i != solut.Count - 1)
                {
                    final_string += solut[i] + ";";
                }
                else
                {
                    final_string += solut[i];
                }
            }
            //wynikowy string:
            //ROUTE #1: 1 2 3 55;ROUTE #2: 2 3 4 66;TOTAL_COST: 777
            //ostatnia liczba w ROUTE to koasz dla danej drogi
            Solution = DataSerialization.GetBytes(final_string);
            if (final_cost != 0) SolutionsMergingFinished(new EventArgs(), this);
        }

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            string partialDataString = DataSerialization.GetString(partialData);
            DVRPParser parser = new DVRPParser();
            ProblemData partialProblemData = parser.Parse(partialDataString);

            List<int> path = partialProblemData.Path;
            List<Point> points = partialProblemData.Locations as List<Point>;
     
            Algorithms tsp = new Algorithms(points);
            tsp.Run(ref path);
            double min_cost = tsp.RouteDistance(path);
           
            Route route = new Route();
            route.RouteID = partialProblemData.VehicleID;
            route.Cost = min_cost;
            route.Locations = path;

            SolutionsMergingFinished(new EventArgs(), this);
            return DataSerialization.GetBytes(parser.ParseRoute(route));
        }
        

    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using ASD.Graph;
using DvrpUtils.ProblemDataModel;
using UCCTaskSolver;

namespace DvrpUtils
{
    class DVRPTaskSolver : TaskSolver
    {
        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override event UnhandledExceptionEventHandler ErrorOccured;

        public override event TaskSolver.ComputationsFinishedEventHandler ProblemDividingFinished;

        public override event TaskSolver.ComputationsFinishedEventHandler ProblemSolvingFinished;

        public override event TaskSolver.ComputationsFinishedEventHandler SolutionsMergingFinished;

        public DVRPTaskSolver(byte[] problemData) : base(problemData) { }

        public override byte[][] DivideProblem(int threadCount)
        {
            //TODO remove temp data
            int duration = 20;
            int vehicles = 15;
            var customers = new List<Customer>
            {
                new Customer{CustomerId = 1, Location = new Point(84, -93), StartDate = 615},
                new Customer{CustomerId = 2, Location = new Point(92, -93), StartDate = 222},
                new Customer{CustomerId = 3, Location = new Point(84, -93), StartDate = 433},
                new Customer{CustomerId = 4, Location = new Point(84, -93), StartDate = 343},
                new Customer{CustomerId = 5, Location = new Point(84, -93), StartDate = 342}
            };
            customers = customers.OrderBy(customer => customer.StartDate).ToList();
            var depot = new Depot { DepotId = 0, Location = new Point(0, 0) };

            //TODO pass only problem instance
            var graphs = CreateDummyGraphs(threadCount, duration, vehicles, depot, customers);

            //TODO create file

            //TODO serialize to byteArray
            if (ProblemDividingFinished != null) ProblemDividingFinished(new EventArgs(), this);
            return null;

        }

        public override void MergeSolution(byte[][] solutions)
        {
            throw new NotImplementedException();
        }

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            DataSerialization.GetString(partialData);
            return null;
        }

        private IEnumerable<IGraph> CreateDummyGraphs(int threadCount, int duration, int vehicles, Depot depot, IEnumerable<Customer> customers)
        {
            var customerList = customers as IList<Customer> ?? customers.ToList();
            int givenCustomers = customerList.Count() / threadCount;
            int currentCustomer = 0;
            var graphs = new List<IGraph>();
            for (int k = 0; k < threadCount; k++)
            {
                IGraph graph = new AdjacencyListsGraph(false, givenCustomers);

                for (int i = currentCustomer; i < currentCustomer + givenCustomers; i++)
                    for (int j = 0; j < currentCustomer + givenCustomers; j++)
                    {
                        if (i != j)
                        {

                            var distance =
                                (int)
                                    Math.Sqrt((customerList[i].Location.X - customerList[i].Location.Y) *
                                              (customerList[i].Location.X - customerList[i].Location.Y)
                                              +
                                              (customerList[j].Location.X - customerList[j].Location.Y) *
                                              customerList[j].Location.X - customerList[j].Location.Y);
                            graph.AddEdge(i + 1, j + 1, distance);
                        }
                    }
                for (int i = currentCustomer; i < currentCustomer + givenCustomers; i++)
                {
                    var distance =
                        (int)
                            Math.Sqrt((customerList[i].Location.X - customerList[i].Location.Y) *
                                      (customerList[i].Location.X - customerList[i].Location.Y)
                                      + (depot.Location.X - depot.Location.Y) * (depot.Location.X - depot.Location.Y));
                    graph.AddEdge(i + 1, 0, distance);
                }

                currentCustomer += givenCustomers;
                graphs.Add(graph);
            }
            return graphs;
        }
        
    }
}

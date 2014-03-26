using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class SolutionRequestStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, MessageType messageType, TimeSpan timeout, EndPoint endPoint) //TODO: add timeout handling when not finished comupting
        {
            SolutionRequest request = MessageSerialization.Deserialize<SolutionRequest>(message);

            DvrpProblem.WaitEvent.WaitOne();

            if (!DvrpProblem.Problems.ContainsKey(request.Id))
            {
                DvrpProblem.WaitEvent.Set();
                return;
            }
            if (DvrpProblem.ProblemSolutions.ContainsKey(request.Id))
            {
                Solutions solution = DvrpProblem.ProblemSolutions[request.Id];
                DvrpProblem.ProblemSolutions.Remove(request.Id);
                ServerNetworkAdapter.Send(stream, solution);
                DvrpProblem.WaitEvent.Set();
                return;
            }

            SolveRequest problem = DvrpProblem.Problems[request.Id];
            Solutions response = new Solutions();
            response.Id = request.Id;
            response.ProblemType = problem.ProblemType;
            response.CommonData = problem.Data;
            List<SolutionsSolution> solutionList = new List<SolutionsSolution>();
            if (DvrpProblem.PartialSolutions.ContainsKey(request.Id))
                foreach (var element in DvrpProblem.PartialSolutions[request.Id])
                    solutionList.AddRange(element.Solutions1);

            if (DvrpProblem.PartialProblemsComputing.ContainsKey(request.Id))
                foreach (var element in DvrpProblem.PartialProblemsComputing[request.Id])
                    foreach (var pr in element.PartialProblems)
                    {
                        SolutionsSolution sol = new SolutionsSolution();
                        sol.TaskId = pr.TaskId;
                        sol.Data = pr.Data;
                        sol.Type = SolutionsSolutionType.Ongoing;
                        solutionList.Add(sol);
                    }

            response.Solutions1 = solutionList.ToArray();
            ServerNetworkAdapter.Send(stream, response);
            DvrpProblem.WaitEvent.Set();
        }
    }
}

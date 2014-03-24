using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class SolutionRequestStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, Common.MessageType messageType, DateTime timeout, ref ulong id, out bool keepAlive, out System.Threading.AutoResetEvent waitEvent)
        {
            SolutionRequest request = MessageSerialization.Deserialize<SolutionRequest>(message);            
            waitEvent = null;
            keepAlive = false;

            if (!DvrpProblem.Problems.ContainsKey(request.Id))
                return;
            if (DvrpProblem.ProblemSolutions.ContainsKey(request.Id))
            {
                Solutions solution = DvrpProblem.ProblemSolutions[request.Id];
                ServerNetworkAdapter.Send(stream, solution);
                return;
            }

            SolveRequest problem = DvrpProblem.Problems[request.Id];
            Solutions response = new Solutions();
            response.Id = request.Id;
            response.ProblemType = problem.ProblemType;
            response.CommonData = problem.Data;
            List<SolutionsSolution> solutionList = new List<SolutionsSolution>();
            foreach (var element in DvrpProblem.PartialSolutions[request.Id])            
                solutionList.AddRange(element.Solutions1);

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
            keepAlive = true;
        }

        public void HandleWaitEvent(System.IO.Stream stream, ulong id)
        {
            throw new NotImplementedException();
        }
    }
}

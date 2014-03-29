using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    public class SolutionRequestStrategy : IMessageStrategy
    {
        /// <summary>
        /// Response for clients solution request
        /// </summary>
        /// <param name="networkAdapter"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="timeout"></param>        
        public void HandleMessage(ServerNetworkAdapter networkAdapter, string message, MessageType messageType, TimeSpan timeout)
        {
            SolutionRequest request = MessageSerialization.Deserialize<SolutionRequest>(message);

            if (request == null)
                return;

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
                networkAdapter.Send(solution);
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
                solutionList.AddRange(DvrpProblem.PartialSolutions[request.Id]);                                            

            if (DvrpProblem.PartialProblemsComputing.ContainsKey(request.Id))
                foreach (var element in DvrpProblem.PartialProblemsComputing[request.Id])                    
                    {
                        SolutionsSolution sol = new SolutionsSolution();
                        sol.TaskId = element.TaskId;
                        sol.Data = element.Data;
                        sol.Type = SolutionsSolutionType.Ongoing;
                        solutionList.Add(sol);                        
                    }

            response.Solutions1 = solutionList.ToArray();
            if (solutionList.Count == 0)
                response.Solutions1 = new SolutionsSolution[] {new SolutionsSolution(),};
            networkAdapter.Send(response);
            DvrpProblem.WaitEvent.Set();
        }
    }
}

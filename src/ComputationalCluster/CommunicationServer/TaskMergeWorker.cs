using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    class TaskMergeWorker
    {
        public static void Work()
        {
            while (true)
            {
                DvrpProblem.TaskMergeEvent.WaitOne();
                DvrpProblem.WaitEvent.WaitOne();

                if (DvrpProblem.PartialSolutions.Count > 0 && DvrpProblem.Tasks.Count > 0)
                {
                    var request = DvrpProblem.PartialSolutions.First();
                    SolveRequest problem = DvrpProblem.Problems[request.Key];
                    Solutions response = new Solutions();
                    response.Id = request.Key;
                    response.ProblemType = problem.ProblemType;
                    response.CommonData = problem.Data;
                    if (request.Value != null)
                        response.Solutions1 = request.Value.ToArray();
                    else
                        response.Solutions1 = new SolutionsSolution[] {new SolutionsSolution(),};
                                            
                    foreach (var task in DvrpProblem.Tasks)
                        ServerNetworkAdapter.Send(DvrpProblem.ComponentsAddress[task.Key], response);  
                }

                DvrpProblem.WaitEvent.Set();
            }
        }
    }
}

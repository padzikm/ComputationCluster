using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    class TaskDivideWorker
    {
        public static void Work()
        {
            while (true)
            {
                DvrpProblem.TaskDivideEvent.WaitOne();
                DvrpProblem.WaitEvent.WaitOne();

                if (DvrpProblem.Problems.Count > 0 && DvrpProblem.Tasks.Count > 0)
                {
                    var pr = DvrpProblem.Problems.First();
                    DivideProblem div = new DivideProblem();
                    div.Id = pr.Key;
                    div.ProblemType = pr.Value.ProblemType;
                    div.Data = pr.Value.Data;
                    div.ComputationalNodes = (ulong)DvrpProblem.Nodes.Count;
                    foreach (var task in DvrpProblem.Tasks)                    
                        ServerNetworkAdapter.Send(DvrpProblem.ComponentsAddress[task.Key], div);                                        
                }

                DvrpProblem.WaitEvent.Set();
            }
        }
    }
}

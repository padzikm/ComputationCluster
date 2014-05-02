using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationServer
{
    public class InactiveComponentCollector
    {
        private Thread currentThread;
        private TimeSpan timeout;
        private TimeSpan delayTime;
        private TimeSpan sleepTime;

        public InactiveComponentCollector(TimeSpan timeout, TimeSpan delayTime, TimeSpan sleepTime)
        {
            this.timeout = timeout;
            this.delayTime = delayTime;
            this.sleepTime = sleepTime;
        }

        public void Start()
        {
            if(currentThread != null)
                throw new InvalidOperationException("Collector is already running!");

            //currentThread = new Thread(Work);
            //currentThread.Start();
        }

        public void Stop()
        {
            currentThread.Abort();
            currentThread = null;
        }

        private void Work()
        {
            while(true)
            {
                Thread.Sleep(sleepTime);

                DvrpProblem.WaitEvent.WaitOne();

                DateTime currentDate = DateTime.UtcNow;
                IEnumerable<ulong> keys = DvrpProblem.ComponentsLastStatus.Keys;
               
                foreach(var key in keys)
                    if((currentDate - DvrpProblem.ComponentsLastStatus[key]) > (timeout + delayTime))
                    {
                        DvrpProblem.ComponentsID.Remove(key);
                        DvrpProblem.ComponentsLastStatus.Remove(key);
                        if(DvrpProblem.Nodes.ContainsKey(key))
                        {
                            DvrpProblem.Nodes.Remove(key);

                            foreach (var problemList in DvrpProblem.PartialProblems.Values)
                                for (int i = 0; i < problemList.Count; ++i)
                                    if (problemList[i].Value == key)
                                        problemList[i] = new KeyValuePair<SolvePartialProblemsPartialProblem, ulong>(problemList[i].Key, 0);
                        }
                        else
                        {
                            DvrpProblem.Tasks.Remove(key);
                            DvrpProblem.ProblemsDividing.Remove(key);
                            DvrpProblem.SolutionsMerging.Remove(key);
                        }
                    }

                DvrpProblem.WaitEvent.Set();
            }
        }
    }
}

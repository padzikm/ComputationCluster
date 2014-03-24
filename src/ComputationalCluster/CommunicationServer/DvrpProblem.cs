using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    [Synchronization]
    static class DvrpProblem
    {
        public static List<ulong> IDList { get; set; }
        public static Dictionary<ulong, DateTime> ComponentsLastStatus { get; set; } 
        public static Dictionary<ulong, Register> Tasks { get; set; }
        public static Dictionary<ulong, Socket> TasksSockets { get; set; }
        public static Dictionary<ulong, bool> TasksBusy { get; set; }
        public static Dictionary<ulong, Register> Nodes { get; set; }
        public static Dictionary<ulong, Socket> NodesSockets { get; set; }
        public static Dictionary<ulong, bool> NodesBusy { get; set; }
        public static Dictionary<ulong, SolveRequest> Problems { get; set; }
        public static Dictionary<ulong, bool> ProblemsDivideWaiting { get; set; }
        public static Dictionary<ulong, bool> ProblemsComputeWaiting { get; set; }
        public static Dictionary<ulong, bool> ProblemsMergeWaiting { get; set; }
        public static Dictionary<ulong, List<SolvePartialProblems>> PartialProblems { get; set; }
        public static Dictionary<ulong, List<SolvePartialProblems>> PartialProblemsComputing { get; set; }
        public static Dictionary<ulong, List<Solutions>> PartialSolutions { get; set; }
        public static Dictionary<ulong, Solutions> ProblemSolutions { get; set; }
        public static AutoResetEvent TaskEvent { get; set; }
        public static AutoResetEvent NodeEvent { get; set; }

        static DvrpProblem()
        {
            IDList = new List<ulong>();
            ComponentsLastStatus = new Dictionary<ulong, DateTime>();
            Tasks = new Dictionary<ulong, Register>();
            TasksSockets = new Dictionary<ulong, Socket>();
            TasksBusy = new Dictionary<ulong, bool>();
            Nodes = new Dictionary<ulong, Register>();
            NodesSockets = new Dictionary<ulong, Socket>();
            NodesBusy = new Dictionary<ulong, bool>();
            Problems = new Dictionary<ulong, SolveRequest>();
            ProblemsDivideWaiting = new Dictionary<ulong, bool>();
            ProblemsComputeWaiting = new Dictionary<ulong, bool>();
            ProblemsMergeWaiting = new Dictionary<ulong, bool>();
            PartialProblems = new Dictionary<ulong, List<SolvePartialProblems>>();
            PartialProblemsComputing = new Dictionary<ulong, List<SolvePartialProblems>>();
            PartialSolutions = new Dictionary<ulong, List<Solutions>>();
            ProblemSolutions = new Dictionary<ulong, Solutions>();
            TaskEvent = new AutoResetEvent(false);
            NodeEvent = new AutoResetEvent(false);
        }

        public static ulong CreateSaveID() //TODO: do better
        {
            ulong id = (ulong)IDList.Count;
            Random r = new Random(DateTime.UtcNow.Millisecond);

            while (IDList.Contains(id))
                id = (ulong)r.Next(0, IDList.Count);
            
            IDList.Add(id);

            return id;
        }
    }
}

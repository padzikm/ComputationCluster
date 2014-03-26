using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{    
    static class DvrpProblem
    {
        public static List<ulong> ComponentsID { get; set; }
        public static List<ulong> ProblemsID { get; set; }
        public static Dictionary<ulong, EndPoint> ComponentsAddress { get; set; } 
        public static Dictionary<ulong, DateTime> ComponentsLastStatus { get; set; } 
        public static Dictionary<ulong, Register> Tasks { get; set; }        
        public static Dictionary<ulong, bool> TasksBusy { get; set; }
        public static Dictionary<ulong, Register> Nodes { get; set; }        
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
        public static AutoResetEvent WaitEvent { get; set; }

        static DvrpProblem()
        {
            ComponentsID = new List<ulong>();
            ProblemsID = new List<ulong>();
            ComponentsAddress = new Dictionary<ulong, EndPoint>();
            ComponentsLastStatus = new Dictionary<ulong, DateTime>();
            Tasks = new Dictionary<ulong, Register>();            
            TasksBusy = new Dictionary<ulong, bool>();
            Nodes = new Dictionary<ulong, Register>();            
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
            WaitEvent = new AutoResetEvent(true);
        }

        public static ulong CreateSaveComponentID() //TODO: do better
        {
            ulong id = (ulong)ComponentsID.Count + 1;
            Random r = new Random(DateTime.UtcNow.Millisecond);

            while (ComponentsID.Contains(id))
                id = (ulong)r.Next(0, ComponentsID.Count);
            
            ComponentsID.Add(id);

            return id;
        }

        public static ulong CreateSaveProblemID() //TODO: do better
        {
            ulong id = (ulong)ProblemsID.Count + 1;
            Random r = new Random(DateTime.UtcNow.Millisecond);

            while (ProblemsID.Contains(id))
                id = (ulong)r.Next(0, ProblemsID.Count);

            ProblemsID.Add(id);

            return id;
        }
    }
}

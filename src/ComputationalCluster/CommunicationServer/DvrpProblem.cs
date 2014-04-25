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
    public static class DvrpProblem
    {
        public static List<ulong> ComponentsID { get; set; }
        public static List<ulong> ProblemsID { get; set; }        
        public static Dictionary<ulong, DateTime> ComponentsLastStatus { get; set; } 
        public static Dictionary<ulong, Register> Tasks { get; set; }                
        public static Dictionary<ulong, Register> Nodes { get; set; }                
        public static Dictionary<ulong, SolveRequest> Problems { get; set; }
        public static Dictionary<ulong, List<ulong>> ProblemsDividing { get; set; } //key = TaskId
        public static Dictionary<ulong, List<KeyValuePair<SolvePartialProblemsPartialProblem, ulong>>> PartialProblems { get; set; }        
        public static Dictionary<ulong, List<SolutionsSolution>> PartialSolutions { get; set; }
        public static Dictionary<ulong, List<ulong>> SolutionsMerging { get; set; } //key = TaskId
        public static Dictionary<ulong, Solutions> ProblemSolutions { get; set; }        
        public static AutoResetEvent WaitEvent { get; set; }

        static DvrpProblem()
        {
            ComponentsID = new List<ulong>();
            ProblemsID = new List<ulong>();            
            ComponentsLastStatus = new Dictionary<ulong, DateTime>();
            Tasks = new Dictionary<ulong, Register>();                        
            Nodes = new Dictionary<ulong, Register>();                        
            Problems = new Dictionary<ulong, SolveRequest>();
            ProblemsDividing = new Dictionary<ulong, List<ulong>>();
            PartialProblems = new Dictionary<ulong, List<KeyValuePair<SolvePartialProblemsPartialProblem, ulong>>>();            
            PartialSolutions = new Dictionary<ulong, List<SolutionsSolution>>();
            SolutionsMerging = new Dictionary<ulong, List<ulong>>();
            ProblemSolutions = new Dictionary<ulong, Solutions>();            
            WaitEvent = new AutoResetEvent(true);
        }

        public static ulong CreateComponentID()
        {
            ulong id = 0;

            for(ulong i = 1; i < (ulong)(ComponentsID.Count + 1); ++i)
                if(!ComponentsID.Contains(i))
                {
                    id = i;
                    break;
                }

            if (id == 0)
                id = (ulong)(ComponentsID.Count + 1);

            return id;
        }

        public static ulong CreateProblemID()
        {
            ulong id = 0;

            for (ulong i = 1; i < (ulong)(ProblemsID.Count + 1); ++i)
                if (!ProblemsID.Contains(i))
                {
                    id = i;
                    break;
                }

            if (id == 0)
                id = (ulong)(ProblemsID.Count + 1);

            return id;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    [Synchronization]
    static class DVRPProblem
    {
        public static List<int> Clients { get; set; } 
        public static List<int> TaskSolvers { get; set; }
        public static List<int> Nodes { get; set; } 
        static DVRPProblem()
        {
            Clients = new List<int>();
            TaskSolvers = new List<int>();
            Nodes = new List<int>();
        }
    }
}

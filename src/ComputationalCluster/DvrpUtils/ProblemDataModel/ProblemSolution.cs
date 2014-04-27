using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class ProblemSolution
    {
        public IEnumerable<Route> Routes { get; set; }
        public int TotalCost { get; set; } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvrpUtils.ProblemDataModel
{
    [Serializable]
    public class Route
    {
        public int RouteID { get; set; }
        public List<int> Locations { get; set; }
    }
}

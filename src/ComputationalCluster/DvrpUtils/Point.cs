using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvrpUtils
{
    public class Point
    {
        private double x;
        private double y;

        public double X 
        {
            get { return x; }
            set { x = value; }
        }
        public double Y 
        {
            get { return y; }
            set { y = value; }
        }

        public Point(double _x, double _y)
        {
            X = _x;
            Y = _y;
        }
    }
}

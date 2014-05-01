using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvrpUtils
{
    class Distances
    {
        Dictionary<Tuple<int, int>, double> Distances = new Dictionary<Tuple<int, int>, double>();

        List<Point> Points;

        public Distances(List<Point> points)
        {
            Points = points;
            Points.Add(points[0]);
            ComputeDistances();
        }

        //TO DO: liczyc tylko polowe wartosci, reszta jest znana
        private void ComputeDistances()
        {
            for (int i = 0; i < Points.Count; ++i)
            {
                for (int j = 0; j < Points.Count; ++j)
                {
                    Tuple<int, int> point = new Tuple<int, int>(i, j);
                    Distances.Add(point, EuclideanDistance(Points[i], Points[j]));
                }
            }
        }

        private double EuclideanDistance(Point p, Point q)
        {
            return Math.Sqrt(Math.Pow(p.X - q.X, 2) + Math.Pow(p.Y - q.Y, 2));
        }

        public double RouteDistance(List<int> points)
        {
            double dist = 0.0;

            int size = points.Count;

            for (int i = 0; i < size - 1; i++)
                dist += GetDistance(points[i], points[i + 1]);

            dist += GetDistance(size - 1, 0);

            return dist;
        }

        public double GetDistance(int i, int j)
        {
            return Distances[new Tuple<int, int>(i, j)];
        }
    }
}

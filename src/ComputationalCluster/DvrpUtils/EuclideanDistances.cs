using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DvrpUtils
{
    public class Distances
    {
        /// <summary>
        /// Tablica odległości pomiędzy każdym możliwym numerem Customer'a
        /// </summary>
        Dictionary<Tuple<int, int>, double> distances = new Dictionary<Tuple<int, int>, double>();

        public Distances(List<Point> points)
        {
            try
            {
                points.Add(points[0]);
                ComputeDistances(points);
            }
            catch(Exception e)
            {
                Console.WriteLine("Probably empty list: " + e.Message);
            }
        }

        /// <summary>
        /// Oblicza odległości euklidesowe dla zbioru punktów Points
        /// </summary>
        /// <param name="Points"></param>
        private void ComputeDistances(List<Point> Points)
        {
            for (int i = 0; i < Points.Count; ++i)
            {
                for (int j = i; j < Points.Count; ++j)
                {
                    Tuple<int, int> point = new Tuple<int, int>(i, j);
                    double actualDist = EuclideanDistance(Points[i], Points[j]);
                    distances.Add(point, actualDist);

                    if (i != j)
                    {
                        Tuple<int, int> pointTranspon = new Tuple<int, int>(j, i);
                        distances.Add(pointTranspon, actualDist);
                    }
                }
            }
        }

        /// <summary>
        /// Liczy odległość euklidesową pomiędzy dwoma punktami.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns> Odległość euklidesowa dla punktów p i q. </returns>
        private double EuclideanDistance(Point p, Point q)
        {
            return Math.Sqrt(Math.Pow(p.X - q.X, 2) + Math.Pow(p.Y - q.Y, 2));
        }

        public double GetDistance(int i, int j)
        {
            return distances[new Tuple<int, int>(i, j)];
        }    
    }
}

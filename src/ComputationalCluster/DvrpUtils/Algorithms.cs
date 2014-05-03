using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DvrpUtils
{
    public class Algorithms
    {
        Dictionary<Tuple<int, int>, double> Distances = new Dictionary<Tuple<int, int>, double>();

        private void WriteLinePoint(List<Point> list)
        {
            foreach (var e in list)
            {
                Console.WriteLine("(" + e.X + "," + e.Y + ")");
            }
        }

        private void WriteLineInt(List<int> list)
        {
            string l = "LISTA: ";
            foreach (var e in list)
            {
                l = l + e + " ";
            }
            Console.WriteLine(l);
        }

        public Algorithms(List<Point> points)
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

        public double Run(ref List<int> points)
        {
            Console.WriteLine("Elements in path to TSP: {0}", points.Count);
            //WriteLineInt(points);
            List<int> temp = PreProcessing(points);
            //WriteLineInt(temp);
            if (temp.Count > 2)
                TwoOpt(ref temp);
            //WriteLineInt(temp);
            points = temp;

            return RouteDistance(points);
        }

        private void ComputeDistances(List<Point> Points)
        {
            for (int i = 0; i < Points.Count; ++i)
            {
                for (int j = i; j < Points.Count; ++j)
                {
                    Tuple<int, int> point = new Tuple<int, int>(i, j);
                    double actualDist = EuclideanDistance(Points[i], Points[j]);
                    Distances.Add(point, actualDist);

                    if (i != j)
                    {
                        Tuple<int, int> pointTranspon = new Tuple<int, int>(j, i);
                        Distances.Add(pointTranspon, actualDist);
                    }
                }
            }
        }

        private double EuclideanDistance(Point p, Point q)
        {
            return Math.Sqrt(Math.Pow(p.X - q.X, 2) + Math.Pow(p.Y - q.Y, 2));
        }

        private double GetDistance(int i, int j)
        {
            return Distances[new Tuple<int, int>(i, j)];
        }

        private double RouteDistance(List<int> points)
        {
            double dist = 0.0;

            int size = points.Count;

            for (int i = 0; i < size - 1; i++)
                dist += GetDistance(points[i], points[i + 1]);

            dist += GetDistance(size - 1, 0);

            return dist;
        }

        private void Swap(ref List<int> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        private void DoTwoOpt(int p1, int p2, int p3, int p4, ref List<int> points)
        {
            if (p3 == p1 || p3 == p2 || p4 == p1 || p4 == p2) return;

            int p1old = points[p1];
            int p2old = points[p2];
            int p3old = points[p3];
            int p4old = points[p4];

            double old_distance = RouteDistance(points);

            Swap(ref points, p2old, p3old);

            double new_distance = RouteDistance(points);

            if (new_distance > old_distance)
            {
                Swap(ref points, p3old, p2old);
            }
        }

        private List<int> PreProcessing(List<int> points)
        {
            List<int> prePath = new List<int>();
            prePath.Add(points[0]);
            int node = 0;

            while (points.Count != prePath.Count)
            {
                node = GetNearestNeighbour(node, points, prePath);
                prePath.Add(node);
            }
            return prePath;
        }

        private void TwoOpt(ref List<int> points)
        {
            points.Add(points[0]);
            int size = points.Count;

            for (int i = 0; i < size - 3; i++)
            {
                for (int j = i + 2; j < size - 1; j++)
                {
                    int p1 = points[i];
                    int p2 = points[i + 1];
                    int p3 = points[j];
                    int p4 = points[j + 1];

                    DoTwoOpt(p1, p2, p3, p4, ref points);
                }
            }
            points.RemoveAt(points.Count - 1);
        }

        private int GetNearestNeighbour(int i, List<int> list, List<int> added)
        {
            int node = 0;
            double min_dist = double.MaxValue;

            foreach (var e in list)
            {
                if (added.Contains(e)) continue;

                double dist = GetDistance(e, i);

                if (dist < min_dist)
                {
                    node = e;
                    min_dist = dist;
                }
            }

            return node;
        }
    }
}

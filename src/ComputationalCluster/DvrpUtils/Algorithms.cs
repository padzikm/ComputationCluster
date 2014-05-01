using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvrpUtils
{
    public class Algorithms
    {
        Distances[] Dist;

        public void WriteLinePoint(List<Point> list)
        {
            foreach (var e in list)
            {
                Console.WriteLine("(" + e.X + "," + e.Y + ")");
            }
        }

        public void WriteLineInt(List<int> list)
        {
            string l = "LISTA: ";
            foreach (var e in list)
            {
                l = l + e + " ";
            }
            Console.WriteLine(l);
        }

        public Algorithms(int _size)
        {
            Dist = new Distances[_size];//(_points);
        }

        public double Run(ref List<int> points, int nr)
        {
            WriteLineInt(points);
            List<int> temp = PreProcessing(points, nr);
            WriteLineInt(temp);
            TwoOpt(ref temp, nr);
            WriteLineInt(temp);
            points = temp;
            return Dist[nr].RouteDistance(points);
        }
        

        public double GetDistance(int i, int j, int nr)
        {
            return Dist[nr].GetDistance(i, j);
        }

        public double RouteDistance(List<int> points, int nr)
        {
            return Dist[nr].RouteDistance(points);
        }

        public void Swap(ref List<int> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public void DoTwoOpt(int p1, int p2, int p3, int p4, ref List<int> points, int nr)
        {
            if (p3 == p1 || p3 == p2 || p4 == p1 || p4 == p2) return;

            int p1old = points[p1];
            int p2old = points[p2];
            int p3old = points[p3];
            int p4old = points[p4];

            double old_distance = Dist[nr].RouteDistance(points);

            Swap(ref points, p2old, p3old);

            double new_distance = Dist[nr].RouteDistance(points);

            if (new_distance > old_distance)
            {
                Swap(ref points, p3old, p2old);
            }
        }

        public List<int> PreProcessing(List<int> points, int nr)
        {
            List<int> prePath = new List<int>();
            prePath.Add(points[0]);
            int node = 0;

            while (points.Count != prePath.Count)
            {
                node = GetNearestNeighbour(node, points, prePath, nr);
                prePath.Add(node);
            }
            return prePath;
        }

        public void TwoOpt(ref List<int> points, int nr)
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

                    DoTwoOpt(p1, p2, p3, p4, ref points, nr);
                }
            }
            points.RemoveAt(points.Count - 1);
        }

        public int GetNearestNeighbour(int i, List<int> list, List<int> added, int nr)
        {
            int node = 0;
            double min_dist = 99999999;

            foreach (var e in list)
            {
                if (added.Contains(e)) continue;

                double dist = Dist[nr].GetDistance(e, i);

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

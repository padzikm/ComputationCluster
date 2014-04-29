using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvrpUtils
{
    class Algorithms
    {
        public double EuclideanDistance(Point p, Point q)
        {
            return Math.Sqrt(Math.Pow(p.X - q.X, 2) + Math.Pow(p.Y - q.Y, 2));
        }

        public double RouteDistance(List<Point> points)
        {
	        double dist = 0.0;

            int size = points.Count;

	        for ( int i = 0; i < size - 1; i++ )
                dist += EuclideanDistance(points[i], points[i + 1]);

            dist += EuclideanDistance(points[size], points[0]);

	        return dist;
        }

        void Swap(ref List<Point> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        void DoTwoOpt(Point p1, Point p2, Point p3, Point p4, ref List<Point> points)
        {
	        if ( p3 == p1 || p3 == p2 || p4 == p1 || p4 == p2 ) return;

            int p1old = points.IndexOf(p1);
            int p2old = points.IndexOf(p2);
            int p3old = points.IndexOf(p3);
            int p4old = points.IndexOf(p4);

	        double old_distance = RouteDistance(points);	
	
            Swap(ref points, p2old, p3old);

            double new_distance = RouteDistance(points);	

	        if ( new_distance > old_distance )
	        {
                Swap(ref points, p3old, p2old);
	        }	
        }

       void TwoOpt(ref List<Point> points)
        {
            points.Add(points[0]);
            int size = points.Count;

            for (int i = 0; i < size - 3; i++)
            {
                for (int j = i + 2; j < size - 1; j++)
                {
                    Point p1 = points[i];
                    Point p2 = points[i + 1];
                    Point p3 = points[j];
                    Point p4 = points[j + 1];

                    DoTwoOpt(p1, p2, p3, p4, ref points);
                }
            }
            points.RemoveAt(points.Count - 1);
        }

        int GetNearestNeighbour(Point p, List<Point> list)
        {
	        int node = 0;

	        double min_dist = 99999999;

	         foreach(var e in list)
	         {
		         if ( list.IndexOf(e) == node ) continue;

                 double dist = EuclideanDistance(p, e);
		
		         if ( dist < min_dist )
		         {
			        node = list.IndexOf(e);
			        min_dist = dist;
		         }
	         }
    
	        return node;
        }
    }
}

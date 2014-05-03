using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvrpUtils
{
   public static class Partitioning {
       public static IEnumerable<List<int>> Partition<T>(List<T> list)
        {
            var partitions = new List<List<int>>();
            Partition(list.Count, list.Count, 0, new int[list.Count], ref partitions);

            return partitions;
        }

        private static void Partition( int n, int m, int ti, int[] helper, ref List<List<int>> list)
        {
            if (n == 0)
            {
                var partition = new List<int>();
                for (int i = 0; i < ti; i++)
                    partition.Add(helper[i]);
                list.Add(partition);
            }
            else
            {
                for (int k = m; k > 0; k--)
                {
                    helper[ti] = k;
                    Partition(n - k, Math.Min(n - k, k), ti + 1, helper, ref list);
                }
            }

        }
    }
}

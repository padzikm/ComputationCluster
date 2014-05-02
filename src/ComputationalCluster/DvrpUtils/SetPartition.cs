using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvrpUtils
{
   public static class Partitioning {
        public static List<T[][]> Partition<T>(List<T> list)
        {
            var partitions = new List<T[][]>();
            Partition(list, list.Count, list.Count, 0, new int[list.Count], ref partitions);

            return partitions;
        }

        private static void Partition<T>(List<T> sourceList, int n, int m, int ti, int[] helper, ref List<T[][]> list)
        {
            if (n == 0)
            {
                var partition = new T[ti][];
                int k = 0;
                for (int i = 0; i < ti; i++)
                {
                    partition[i] = new T[helper[i]];
                    
                    for (int j = 0; j < helper[i]; j++)
                    {
                        partition[i][j] = sourceList[j + k];
                    }
                    k += helper[i];
                }
                list.Add(partition);
            }
            else
            {
                for (int k = m; k > 0; k--)
                {
                    helper[ti] = k;
                    Partition(sourceList, n - k, Math.Min(n - k, k), ti + 1, helper, ref list);
                }
            }

        }
    }
}

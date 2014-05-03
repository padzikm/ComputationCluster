using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvrpUtils
{
    public static class Partitioning
    {
        public static IEnumerable<List<int>> Partition<T>(List<T> list)
        {
            var partitions = new List<List<int>>();
            Partition(list.Count, list.Count, 0, new int[list.Count], ref partitions);

            return partitions;
        }

        private static void Partition(int n, int m, int ti, int[] helper, ref List<List<int>> list)
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

        private static List<List<int>> Combinations(int[] array, int startingIndex = 0, int combinationLenght = 2)
        {

            List<List<int>> combinations = new List<List<int>>();
            if (combinationLenght == 2)
            {

                int combinationsListIndex = 0;
                for (int arrayIndex = startingIndex; arrayIndex < array.Length; arrayIndex++)
                {

                    for (int i = arrayIndex + 1; i < array.Length; i++)
                    {
                        combinations.Add(new List<int>());

                        combinations[combinationsListIndex].Add(array[arrayIndex]);
                        while (combinations[combinationsListIndex].Count < combinationLenght)
                        {
                            combinations[combinationsListIndex].Add(array[i]);
                        }
                        combinationsListIndex++;
                    }

                }

                return combinations;
            }

            List<List<int>> combinationsofMore = new List<List<int>>();
            for (int i = startingIndex; i < array.Length - combinationLenght + 1; i++)
            {
                combinations = Combinations(array, i + 1, combinationLenght - 1);

                foreach (List<int> element in combinations)
                {
                    element.Insert(0, array[i]);
                }

                combinationsofMore.AddRange(combinations);
            }

            return combinationsofMore;
        }
    }
}

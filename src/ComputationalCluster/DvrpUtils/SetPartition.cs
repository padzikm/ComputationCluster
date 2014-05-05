using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DvrpUtils.ProblemDataModel;

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

        public static List<List<T>> Combinations<T>(T[] array, int startingIndex = 0, int combinationLenght = 2)
        {

            List<List<T>> combinations = new List<List<T>>();
            if (combinationLenght == 2)
            {

                int combinationsListIndex = 0;
                for (int arrayIndex = startingIndex; arrayIndex < array.Length; arrayIndex++)
                {

                    for (int i = arrayIndex + 1; i < array.Length; i++)
                    {
                        combinations.Add(new List<T>());

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
            if (combinationLenght == 1)
            {
                foreach (var element in array)
                {
                    var list = new List<T> { element };
                    combinations.Add(list);
                }
                return combinations;
            }

            List<List<T>> combinationsofMore = new List<List<T>>();
            for (int i = startingIndex; i < array.Length - combinationLenght + 1; i++)
            {
                combinations = Combinations(array, i + 1, combinationLenght - 1);

                foreach (List<T> element in combinations)
                {
                    element.Insert(0, array[i]);
                }

                combinationsofMore.AddRange(combinations);
            }

            return combinationsofMore;
        }

        public static void GenerateValidProblems(List<int> partition, IEnumerable<Customer> customers, int number,
            out List<List<List<Customer>>> allCombinations)
        {
            allCombinations = new List<List<List<Customer>>>();
            var combinations = Combinations(customers.ToArray(), 0, partition[number]);
            foreach (var combination in combinations)
            {
                var list = new List<List<Customer>> { combination };
                allCombinations.Add(list);
            }
            number++;
            for (int i = number; i < partition.Count; i++)
            {


                combinations = Combinations(customers.ToArray(), 0, partition[i]);
                for (int j = 0; j < allCombinations.Count; j++)
                {
                    var allCombination = allCombinations[j];
                    bool found = false;
                    for (int k = 0; k < combinations.Count; k++)
                    {
                        var combination = combinations[k];

                        for (int p = 0; p < allCombination.Count; p++)
                        {
                            var list = allCombination[p];

                            if (!list.Intersect(combination).Any())
                            {
                                allCombination.Add(combination);
                                found = true;
                                break;
                            }
                        }
                        if (found)
                            break;
                        
                    }
                }

            }

        }

    }
}

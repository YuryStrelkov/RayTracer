using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raytracer.Tree
{
    /// <summary>
    /// Методы расширения для IList<T>
    /// </summary>
    public static class ArrayUtils
    {
        public static void ChangeEach<T>(this IList<T> array, Func<T, T> mutator)
        {
            
            Parallel.For(0, array.Count, index =>
            {
                array[index] = mutator(array[index]);
            });
        }

        public static void ChangeEach<T>(this IList<T> array, Func<T, T> mutator, int ofset)
        {
            if (array.Count % ofset!=0)
            {
                return;
            }

            Parallel.For(0, array.Count/ofset, index =>
            {
                array[index * ofset]     = mutator(array[index * ofset]);
                array[index * ofset + 1] = mutator(array[index * ofset + 1]);
                array[index * ofset + 1] = mutator(array[index * ofset + 2]);
            });
        }

        public static void FillByPattern<T>(this IList<T> array, T[] pattern)
        {
            if (array.Count % pattern.Length != 0)
            {
                return;
            }

            Parallel.For(0, array.Count / pattern.Length, index =>
            {
                int idx = index * pattern.Length;

                for (int i=0;i<pattern.Length ;i++)
                {
                    array[idx + i] = pattern[i];
                }
               
            });
        }


        /// https://stackoverflow.com/questions/1897458/parallel-sort-algorithm
        /// То что ниже, скорее всего работать не будет, нужно проверить работоспособность кода из ссылки сверху для сортировки 
        public static void SortAscend<T>(this IList<T> array, int ofset) where T:IComparable
        {
            Parallel.For(0, array.Count - 1, index =>
            {
                if (array[index].CompareTo(array[index + 1]) < 0)
                {
                    array.Swap(index, index + 1);
                }
            });
        }

        public static void SortDescend<T>(this IList<T> array, int ofset) where T : IComparable
        {
            Parallel.For(0, array.Count - 1, index =>
            {
                if (array[index].CompareTo(array[index + 1]) > 0)
                {
                    array.Swap(index, index + 1);
                }
            });
        }

        static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

    }
}

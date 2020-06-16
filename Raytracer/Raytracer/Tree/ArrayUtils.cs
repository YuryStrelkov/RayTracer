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
        public static void ForEach<T>(this IList<T> array, Func<T, T> mutator)
        {
            
            Parallel.For(0, array.Count, index =>
            {
                array[index] = mutator(array[index]);
            });
        }

        public static void ChangeEach<T>(this IList<T> array, Func<T, T> mutator, int ofset)
        {
            Parallel.For(0, array.Count, index =>
            {
                array[index] = mutator(array[index]);
                index += ofset;
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

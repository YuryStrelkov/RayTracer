using Raytracer.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class BinaryCompater : ICompater<double>
    {
        public bool Compare(ref double left, ref double right)
        {
            return left  < right ;
        }

        public long CreateUniqueID(ref double data)
        {
            return data.GetHashCode();
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            BTree<double> tree = new BTree<double>(0);
            tree.SetCompater(new BinaryCompater());
            for (int i=1; i<10; i++)
            {
                tree.Add(i);
            }
            Console.WriteLine(tree.ToString());
            Console.ReadKey();
        }
    }
}

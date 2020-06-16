using Raytracer;
using Raytracer.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    class Program
    {
        static void Main(string[] args)
        {
            BTree<double> tree = TreeBuilder.CreateDoubleBinaryTree();
            for (int i = 1; i < 10; i++)
            {
                tree.Add(i);
            }

            Console.WriteLine(tree.ToString());
            Console.ReadKey();
        }
    }
}

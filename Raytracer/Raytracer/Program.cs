using Raytracer;
using Raytracer.Tree;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Raytracer
{
    class Program
    {
        public static int StatmentCount = 0;

        public static int MaxTasks = 100;

        public static Mutex mutex = new Mutex();

        public static object mutex_=new object();// = new Mutex();

        static Task TaskTest()
        {
            Task t =  Task.Run(()=> 
            {
                Random r = new Random();

                Task.Delay((int)(r.NextDouble() * 1100));

                lock(mutex_)
                {
                    StatmentCount++;
                    Console.WriteLine("Run task : " + StatmentCount);
                }
                //mutex.ReleaseMutex();
           });
            return t;
        }

        static Task TaskTestCountCheck()
        {
            Task t = Task.Run(() =>
            {
                while ( StatmentCount != MaxTasks)
                {

                }
                Console.WriteLine("All tasks done");
            }
            );
            return t;
        }

        static async void TaskTestAsync()
        {
           await TaskTest();
        }

        static async void TaskTestCountCheckAsync()
        {
            await TaskTestCountCheck();
        }


        static void Main(string[] args)
        {
            TaskTestCountCheckAsync();

            for (int i = 0; i < MaxTasks; i++)
            {
                TaskTestAsync();
            }

            Console.ReadKey();
          
        }
    }
}

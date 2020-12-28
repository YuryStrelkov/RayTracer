
using System.Collections.Generic;

namespace Raytracer.Model.Nodes
{
    public class InstanceCounter
    {
        private int InstanceCount = 0;

        private object mutex;

        private Stack<int> deletedInstances;

        public void RemoveInstance(int id)
        {
            lock (mutex)
            {
                InstanceCount--;
                deletedInstances.Push(id);
            }
        }

        public int GetInstanceIdAvailable()
        {

            lock (mutex)
            {
                if (deletedInstances.Count != 0)
                {
                    return deletedInstances.Peek();
                }

                return InstanceCount + 1;
            }
        }

        public int GetInstanceId()
        {
            lock (mutex)
            {
                if (deletedInstances.Count != 0)
                {
                    InstanceCount++;
                    ///  Console.WriteLine("Instance ID : " + deletedInstances.Peek());
                    return deletedInstances.Pop();
                }
                //Console.WriteLine("Instance ID : " + (InstanceCount + 1));
                return InstanceCount++;
            }
        }

        public InstanceCounter()
        {
            mutex = new object();
            deletedInstances = new Stack<int>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Raytracer.Model.Nodes
{

    public interface ICopy<T>
    {
        T Copy();
    }

    public interface INodeProcess<T, ResultType> where T : ICopy<T>
    {
        ResultType GetProcessResult();

        void OnStart(int level, Node<T> n);

        void Process(int level, Node<T> n);

        void OnEnd(int level, Node<T> n);
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Node<T> : IDisposable, IEquatable<Node<T>>, ICopy<Node<T>> where T : ICopy<T>
    {
        /// <summary>
        /// Счётчик количества созданных объектов на данный момент
        /// </summary>
        private static readonly InstanceCounter instanceCounter = new InstanceCounter();
        /// <summary>
        /// Родительский ID
        /// </summary>
        private int parentId;
        /// <summary>
        /// Собственный ID
        /// </summary>
        private int Id;
        /// <summary>
        /// ID дочерних элементов
        /// </summary>
        private Dictionary<int, int> childrens;
        /// <summary>
        /// Хранимые данные
        /// </summary>
        private T data;
        /// <summary>
        /// Метод доступа для обращения к хранимым данным
        /// </summary>
        /// <returns>хранимые данные</returns>
        public T GetData()
        {
            return data;
        }
        /// <summary>
        /// Метод доступа для получения собственного ID
        /// </summary>
        /// <returns>собственный ID</returns>
        public int GetID()
        {
            return Id;
        }
        /// <summary>
        /// Метод доступа для установки родительского нода
        /// </summary>
        /// <param name="n">новый родительский нод</param>
        public void SetParentID(Node<T> n)
        {
            parentId = n.GetID();
        }
        /// <summary>
        /// Метод доступа для установки родительского ID
        /// </summary>
        /// <param name="id">новый родительский ID</param>
        public void SetParentID(int id)
        {
            parentId = id;
        }

        public int GetParentID()
        {
            return parentId;
        }

        public int[] GetChildrenIDs()
        {
            return childrens.Keys.ToArray();
        }

        public Dictionary<int, int> GetChildren()
        {
            return childrens;
        }

        public void RemoveChild(int id)
        {
            if (!childrens.ContainsKey(id))
            {
                return;
            }
            childrens.Remove(id);
        }

        public void AddChild(Node<T> child)
        {
            AddChild(child.GetID());
            child.parentId = GetID();
        }

        public void AddChild(int id)
        {
            // исключение зацикливания
            if (id == GetID())
            {
                return;
            }

            if (childrens.ContainsKey(id))
            {
                return;
            }
            childrens.Add(id, id);
        }

        public Node<T> Copy()
        {
            return new Node<T>(this);
        }

        public bool Equals(Node<T> n)
        {
            if (n.parentId != parentId)
            {
                return false;
            }

            if (!n.data.Equals(data))
            {
                return false;
            }

            if (!n.childrens.Equals(childrens))
            {
                return false;
            }
            return true;
        }

        private Node(Node<T> n)
        {
            data = n.data.Copy();

            parentId = n.parentId;

            childrens = new Dictionary<int, int>(n.childrens);

            Id = instanceCounter.GetInstanceId();
        }

        public Node(int parentID, T data_)
        {

            data = data_;

            parentId = parentID;

            childrens = new Dictionary<int, int>();

            Id = instanceCounter.GetInstanceId();
        }

        public Node(T data_)
        {

            data = data_;

            parentId = -1;

            childrens = new Dictionary<int, int>();

            Id = instanceCounter.GetInstanceId();

        }

        public override string ToString()
        {
            return "ID : " + GetID() + " data : " + data.ToString();
        }

        public void Dispose()
        {
            instanceCounter.RemoveInstance(Id);
            GC.SuppressFinalize(this);
        }

        /*~Node()
        {
         instanceCounter.removeInstance(Id);
        }*/

    }
}

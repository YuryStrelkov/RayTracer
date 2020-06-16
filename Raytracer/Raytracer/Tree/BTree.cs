using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Raytracer.Tree
{
    public class BinaryDoubleCompater : ICompater<double>
    {
        public bool Compare(ref double left, ref double right)
        {
            return left < right;
        }

        public long CreateUniqueID(ref double data)
        {
            return data.GetHashCode();
        }
    }

    public class BinaryFloatCompater : ICompater<float>
    {
        public bool Compare(ref float left, ref float right)
        {
            return left < right;
        }

        public long CreateUniqueID(ref float data)
        {
            return data.GetHashCode();
        }
    }

    public class BinaryIntCompater : ICompater<int>
    {
        public bool Compare(ref int left, ref int right)
        {
            return left < right;
        }

        public long CreateUniqueID(ref int data)
        {
            return data.GetHashCode();
        }
    }

    public interface IProcess<T>
    {
        void Process(T data);
    }

    public interface ICompater<T>
    {
       bool Compare(ref T left, ref T right);
       long CreateUniqueID(ref T data);
    }

    public class DefaultCompater<T> : ICompater<T>
    {
        public bool Compare(ref T left, ref T right)
        {
            return  left.GetHashCode() < right.GetHashCode();
        }

        public long CreateUniqueID(ref T data)
        {
            return data.GetHashCode();
        }

    }

    public static class TreeBuilder
    {
        public static BTree<float> CreateFloatBinaryTree()
        {
            BTree<float> btree = new BTree<float>();
            btree.SetCompater(new BinaryFloatCompater());
            return btree;
        }

        public static BTree<double> CreateDoubleBinaryTree()
        {
            BTree<double> btree = new BTree<double>();
            btree.SetCompater(new BinaryDoubleCompater());
            return btree;
        }

        public static BTree<int> CreateIntegerBinaryTree()
        {
            BTree<int> btree = new BTree<int>();
            btree.SetCompater(new BinaryIntCompater());
            return btree;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class BTree<T> : IEquatable<BTree<T>>, IEnumerable 
    {
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        protected struct Node : IEquatable<Node>
        {
            public new string ToString()
            {
                return "ID : " + ID + "; PARENT :" + ParentID + "; LEFT : " + LeftID + "; RIGHT : " + RightID + "; DATA : " + Data.ToString();
            }
            
            public bool Equals(Node n)
            {
                if (ID != n.ID)
                {
                    return false;
                }
                if (ParentID != n.ParentID)
                {
                    return false;
                }
                if (LeftID != n.LeftID)
                {
                    return false;
                }
                if (RightID != n.RightID)
                {
                    return false;
                }

                if (!Data.Equals(n.Data))
                {
                    return false;
                }
                return true;
            }

            public long ID { get; set; }

            public long ParentID { get; set; }

            public long LeftID { get; set; }

            public long RightID { get; set; }

            public T Data { get; set; }
        }

        private long HashCode;

        public long Depth{ get; protected set; }

        public long RootID { get; protected set;}
        
        private Dictionary<long, Node> Nodes;

        private ICompater<T> TreeComparer;
        
        public void Add(T elem)
        {
            if (RootID == -1)
            {
                RootID = TreeComparer.CreateUniqueID(ref elem);

                Node n = new Node
                {
                    ID = RootID,
                    ParentID = RootID,
                    LeftID = -1,
                    RightID =-1,
                    Data = elem
                 };
                AddNodeDirect(n);
                HashCode = HashCode ^ elem.GetHashCode();
                return;
            }
            RecursiveAdd( 0, RootID, elem);
            HashCode = HashCode ^ elem.GetHashCode();
        }

        public T Get(long id)
        {
            return Nodes[id].Data;
        }

        public void Remove(long id)
        {
            if (!Nodes.ContainsKey(id))
            {
                return;
            }

            if (Nodes[id].LeftID == -1 && Nodes[id].RightID == -1)
            {
                Nodes.Remove(id);
                return;
            }

            Node parent = GetParent(Nodes[id]);
            /// Если правая ветка
            if (parent.RightID == id)
            {
                /// Если у удаляемого нет левых детей
                
                if (Nodes[id].LeftID == -1)
                {
                    parent.RightID = Nodes[id].RightID;

                    Nodes[parent.ID] = parent;

                    Nodes.Remove(id);

                    return;
                }
                /// Если у удаляемого есть правые дети
                if (Nodes[id].RightID != -1)
                {
                    parent.RightID = Nodes[id].LeftID;

                    Node rightest = RightestNode(Nodes[id].LeftID);

                    rightest.RightID = Nodes[id].RightID;

                    Nodes[parent.ID] = parent;

                    Nodes[rightest.ID] = rightest;

                    Nodes.Remove(id);

                    return;
                }
                Nodes.Remove(id);

                return;

            }
            /// Если левая ветка
            if (parent.LeftID == id)
            {
                if (Nodes[id].LeftID == -1)
                {
                    parent.LeftID = Nodes[id].RightID;

                    Nodes[parent.ID] = parent;

                    Nodes.Remove(id);

                    return;
                }

                if (Nodes[id].RightID != -1)
                {
                    parent.LeftID = Nodes[id].LeftID;

                    Node rightest = RightestNode(Nodes[id].LeftID);

                    rightest.RightID = Nodes[id].RightID;

                    Nodes[parent.ID] = parent;

                    Nodes[rightest.ID] = rightest;

                    Nodes.Remove(id);

                    return;
                }

                //// Удаляем корень 
                Node leftest;

                if (Nodes[id].RightID!=-1)
                {
                    leftest = LeftestNode(Nodes[id].RightID);

                    leftest.LeftID = Nodes[id].LeftID;

                    Nodes[leftest.ID] = leftest;
                }

                RootID = Nodes[id].RightID;

                Nodes.Remove(id);

                return;
            }


            
        }

        public override int GetHashCode()
        {
            return (int)HashCode;
        }

        public void PreOrderTraversal(IProcess<T> action)
        {
            PreOrderTraversal(action, RootID);
        }

        public void InOrderTraversal(IProcess<T> action)
        {
            InOrderTraversal(action, RootID);
        }

        public void PostOrderTraversal(IProcess<T> action)
        {
            PostOrderTraversal(action, RootID);
        }

        /// <summary>
        /// Напрямую добавляет новый нод, создавая его новый экземпляр
        /// </summary>
        /// <param name="ParentID">родительский нод</param>
        /// <param name="nodeID">уникальный идентификатор добавляемого нода</param>
        /// <param name="data_">хранимая информация</param>
   
        public void SetCompater(ICompater<T> comparer)
        {
            TreeComparer = comparer;
        }

        public void Merge(BTree<T> tree)
        {
            foreach (long id in tree.Nodes.Keys)
            {
                AddNode(tree.Nodes[id]);
            }
        }

        public override string ToString()
        {
            return RecursiveToString(0,RootID);
        }

        public bool Equals(BTree<T> tree)
        {
             return RecursiveEquals(RootID,ref tree);
        }
        
        public BTree<T> GetSubTreeCopy(long nodeID)
        {
            BTree<T> refTree = new BTree<T>();
           
            refTree.RootID = nodeID;

            refTree.AddNodeDirectCopy(Nodes[nodeID]);

            return refTree;
        }

        public BTree<T> GetSubTreeRef(long nodeID)
        {
            BTree<T> refTree = new BTree<T>();             

            refTree.RootID = nodeID;

            refTree.AddNodeDirect(Nodes[nodeID]);

            RecursiveGetSubTreeRef(ref refTree, nodeID);

            return refTree;
        }

        public IEnumerator GetEnumerator()
        {
            return InOrderTraversal();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator InOrderTraversal()
        {
            if (RootID != -1)
            {
                // Стек для сохранения пропущенных узлов.
                Stack<long> stack = new Stack<long>();

                Node current = Nodes[RootID];

                bool goLeftNext = true;

                stack.Push(current.ID);

                while (stack.Count > 0)
                {
                    if (goLeftNext)
                    {
                        while (current.LeftID != -1)
                        {
                            stack.Push(current.ID);
                            current = Nodes[current.LeftID];
                        }
                    }

                    yield return current.Data;

                    if (current.RightID != -1)
                    {
                        current = Nodes[current.RightID];

                        goLeftNext = true;
                    }
                    else
                    {
                        current = Nodes[stack.Pop()];
                        goLeftNext = false;
                    }
                }
            }
        }

        private string RecursiveToString(int level, long id)
        {
            string tab1 = new string('\t', level);
            string tab2 = new string('\t', level + 1);
            string result = tab1 + "Node ID : " + id.ToString() + " Value : " + Nodes[id].Data.ToString() +
                            "\n" + tab2 + "Left  ID : " + Nodes[id].LeftID.ToString() +
                            "\n" + tab2 + "Right ID : " + Nodes[id].RightID.ToString() + "\n";
            if (Nodes[id].LeftID != -1)
            {
                result += RecursiveToString(level + 1, Nodes[id].LeftID);
            }

            if (Nodes[id].RightID != -1)
            {
                result += RecursiveToString(level + 1, Nodes[id].RightID);
            }
            return result;
        }

        private void PostOrderTraversal(IProcess<T> action, long id)
        {
            if (id != -1)
            {
                PostOrderTraversal(action, Nodes[id].LeftID);

                PostOrderTraversal(action, Nodes[id].RightID);

                action.Process(Nodes[id].Data);
            }
        }

        private void PreOrderTraversal(IProcess<T> action, long id)
        {
            if (id != -1)
            {
                action.Process(Nodes[id].Data);

                PreOrderTraversal(action, Nodes[id].LeftID);

                PreOrderTraversal(action, Nodes[id].RightID);
            }
        }

        private void InOrderTraversal(IProcess<T> action, long id)
        {
            if (id != -1)
            {
                InOrderTraversal(action, Nodes[id].LeftID);

                action.Process(Nodes[id].Data);

                InOrderTraversal(action, Nodes[id].RightID);
            }
        }

        private void recursiveAddNode(long level, long nodeID, Node node)
        {
            T nodeData = Nodes[nodeID].Data;

            T nodeData2Add = Nodes[nodeID].Data;

            if (TreeComparer.Compare(ref nodeData, ref nodeData2Add))
            {
                if (Nodes[nodeID].RightID == -1)
                {
                    node.ParentID = nodeID; // Nodes[nodeID].RightID;

                    Node n = Nodes[nodeID];

                    n.RightID = node.ID;

                    Nodes[nodeID] = n;

                    Nodes.Add(node.ID, node);
                    if (level > Depth)
                    {
                        Depth = level;
                    }
                    return;
                }
                recursiveAddNode(level + 1, Nodes[nodeID].RightID, node);
                return;// я хз нужен он тут или нет, но пусть будет
            }
            if (Nodes[nodeID].LeftID == -1)
            {
                node.ParentID = nodeID; // Nodes[nodeID].RightID;

                Node n = Nodes[nodeID];

                n.LeftID = node.ID;

                Nodes[nodeID] = n;

                Nodes.Add(node.ID, node);

                if (level > Depth)
                {
                    Depth = level;
                }
                return;
            }
            recursiveAddNode(level + 1, Nodes[nodeID].LeftID, node);
        }

        private bool RecursiveEquals(long NodeID, ref BTree<T> tree)
        {
            if (!tree.Nodes.ContainsKey(NodeID))
            {
                return false;
            }

            return RecursiveEquals(tree.Nodes[NodeID].LeftID, ref tree)
                 & RecursiveEquals(tree.Nodes[NodeID].RightID, ref tree);
        }

        private void RecursiveGetSubTreeCopy(ref BTree<T> refTree, long nodeID)
        {
            if (Nodes[nodeID].LeftID == -1)
            {
                refTree.AddNodeDirectCopy(Nodes[Nodes[nodeID].LeftID]);
                RecursiveGetSubTreeCopy(ref refTree, Nodes[nodeID].LeftID);
            }

            if (Nodes[nodeID].RightID == -1)
            {
                refTree.AddNodeDirectCopy(Nodes[Nodes[nodeID].RightID]);
                RecursiveGetSubTreeCopy(ref refTree, Nodes[nodeID].RightID);
            }
        }

        private void RecursiveGetSubTreeRef(ref BTree<T> refTree, long nodeID)
        {
            if (Nodes[nodeID].LeftID == -1)
            {
                refTree.AddNodeDirect(Nodes[Nodes[nodeID].LeftID]);
                RecursiveGetSubTreeRef(ref refTree, Nodes[nodeID].LeftID);
            }

            if (Nodes[nodeID].RightID == -1)
            {
                refTree.AddNodeDirect(Nodes[Nodes[nodeID].RightID]);
                RecursiveGetSubTreeRef(ref refTree, Nodes[nodeID].RightID);
            }
        }

        private void AddNode(long ParentID, long nodeID, T data_)
        {
            if (Nodes.ContainsKey(nodeID))
            {
                return;
            }
            Node n = new Node()
            {
                ID = nodeID,
                ParentID = ParentID,
                LeftID = -1,
                RightID = -1,
                Data = data_
            };
            Nodes.Add(n.ID, n);
        }

        private void AddNode(Node node)
        {
            if (Nodes.ContainsKey(node.ID))
            {
                return;
            }
            recursiveAddNode(0, RootID, node);
        }
        
        private void RecursiveAdd(long level, long nodeID, T data)
        {
            T nodeData = Nodes[nodeID].Data;

            if (TreeComparer.Compare(ref nodeData, ref data))
            {
                if (Nodes[nodeID].RightID == -1)
                {
                    Node n = Nodes[nodeID];

                    n.RightID = TreeComparer.CreateUniqueID(ref data);

                    Nodes[nodeID] = n;
                     
                    AddNode(nodeID, Nodes[nodeID].RightID, data);

                    if (level>Depth)
                    {
                        Depth = level;
                    }
                    return;
                }
                RecursiveAdd(level + 1, Nodes[nodeID].RightID, data);
                return;// я хз нужен он тут или нет, но пусть будет
            }
            if (Nodes[nodeID].LeftID == -1)
            {
                Node n = Nodes[nodeID];

                n.LeftID = TreeComparer.CreateUniqueID(ref data);

                Nodes[nodeID] = n;

                AddNode(nodeID, Nodes[nodeID].LeftID, data);

                if (level > Depth)
                {
                    Depth = level;
                }
                return;
            }
            RecursiveAdd(level + 1, Nodes[nodeID].LeftID, data);
        }
        //Без проверки добавляет узел в Nodes
        private void AddNodeDirect(Node node)
        {
            Nodes.Add(node.ID, node);
        }

        private void AddNodeDirectCopy(Node node)
        {
            Node copy = new Node()
            {
                ID = node.ID,
                ParentID = node.ParentID,
                LeftID = -1,
                RightID = -1,
                Data = GenericDeepCopy<T>.GetDeepCopy(node.Data)
            };

            Nodes.Add(node.ID, node);
        }

        private Node GetParent(Node node)
        {
            long parentID = Nodes[node.ID].ParentID;

            return Nodes[parentID];
        }

        private Node LeftestNode(long startID)
        {
            if (Nodes[startID].LeftID == -1)
            {
                return Nodes[startID];
            }
            return LeftestNode(Nodes[startID].LeftID);
        }

        private Node RightestNode(long startID)
        {
            if (Nodes[startID].RightID == -1)
            {
                return Nodes[startID];
            }
            return RightestNode(Nodes[startID].RightID);
        }

        /// <summary>
        /// Добавляет новый нод в дерево с учётом настройки через IComparater
        /// </summary>
        /// <param name="node"></param>

        public BTree()
        {
            Depth = 0;

            RootID = -1;

            HashCode = 0;

            Nodes = new Dictionary<long, Node>();

            TreeComparer = new DefaultCompater<T>();
        }

        public BTree(T data)
        {
            Depth = 0;

            Nodes = new Dictionary<long, Node>();

            HashCode = data.GetHashCode();

            TreeComparer = new DefaultCompater<T>();

            RootID = TreeComparer.CreateUniqueID(ref data);

            AddNode(RootID, RootID, data);

        }
    }
}

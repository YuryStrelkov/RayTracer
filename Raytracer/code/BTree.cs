using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Raytracer.Tree
{
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

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class BTree<T> : IEquatable<BTree<T>>/// where T:struct
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

        public long Depth{ get; protected set; }

        public long RootID { get; protected set;}
        
        private Dictionary<long, Node> Nodes;

        private ICompater<T> TreeComparer;
        
        public void Add(T elem)
        { 
            RecursiveAdd( 0, RootID, elem);
        }

        public T Get(long id)
        {
            return Nodes[id].Data;
        }
     
        /// <summary>
        /// Напрямую добавляет новый нод, создавая его новый экземпляр
        /// </summary>
        /// <param name="ParentID">родительский нод</param>
        /// <param name="nodeID">уникальный идентификатор добавляемого нода</param>
        /// <param name="data_">хранимая информация</param>
       
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
                recursiveAddNode(level+1,Nodes[nodeID].RightID, node);
                return;// я хз нужен он тут или нет, но пусть будет
            }
            if (Nodes[nodeID].LeftID == -1)
            {
                node.ParentID = nodeID; // Nodes[nodeID].RightID;

                Node n = Nodes[nodeID];

                n.LeftID = node.ID;

                Nodes[nodeID] = n;

                Nodes.Add(node.ID, node);

                if (level>Depth)
                {
                    Depth = level;
                }
                return;
            }
            recursiveAddNode(level + 1, Nodes[nodeID].LeftID, node);
        }
        
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

        private string RecursiveToString(int level, long id)
        {
            string tab1 = new string('\t', level);
            string tab2 = new string('\t', level + 1);
            string result = tab1 + "Node ID : " + id.ToString() + " Value : " +Nodes[id].Data.ToString() +
                            "\n"+ tab2 + "Left  ID : " + Nodes[id].LeftID.ToString()+
                            "\n"+ tab2 + "Right ID : " + Nodes[id].RightID.ToString()+ "\n";
            if (Nodes[id].LeftID!=-1)
            {
                result += RecursiveToString(level + 1, Nodes[id].LeftID);
            }

            if (Nodes[id].RightID != -1)
            {
                result += RecursiveToString(level + 1, Nodes[id].RightID);
            }
            return result;
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
                RecursiveAdd(level+1, Nodes[nodeID].RightID, data);
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
        /// <summary>
        /// Добавляет новый нод в дерево с учётом настройки через IComparater
        /// </summary>
        /// <param name="node"></param>

        protected BTree()
        {
            Depth = 0;

            Nodes = new Dictionary<long, Node>();

            TreeComparer = new DefaultCompater<T>();
        }

        public BTree(T data)
        {
            Depth = 0;

            Nodes = new Dictionary<long, Node>();

            TreeComparer = new DefaultCompater<T>();

            RootID = TreeComparer.CreateUniqueID(ref data);

            AddNode(RootID, RootID, data);

        }
    }
}

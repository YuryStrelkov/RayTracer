using System;
using System.Collections.Generic;

namespace Raytracer.Model.Nodes
{
    public class NodeList<T> : IDisposable where T : ICopy<T>
    {
        public Dictionary<int, Node<T>> Nodes { get; protected set; }

        public int LastID { get; protected set; }

        public int RootID { get; protected set; }

        public bool TryToGetNode(out Node<T> n, int nodeID)
        {
            if (ContainsNode(nodeID))
            {
                n = Nodes[nodeID];

                return true;
            }

            n = new Node<T>();

            return false;
        }

        public bool HasChildrens(int node)
        {
            return Nodes[node].GetChildrenIDs().Length != 0;
        }

        public void RemoveChildrens(int node)
        {
            foreach (int i in Nodes[node].GetChildrenIDs())
            {
                RemoveNode(i);
            }
        }

        public Node<T> GetNode(int nodeID)
        {
            if (Nodes.ContainsKey(nodeID))
            {
                return Nodes[nodeID];
            }
            return Nodes[RootID];
        }

        public T GetNodeData(int nodeID)
        {
            return GetNode(nodeID).GetData();
        }

        public Dictionary<int, Node<T>>.KeyCollection GetNodeKeys()
        {
            return Nodes.Keys;
        }

        protected void AddNodeDirect(Node<T> n)
        {
            if (!ContainsNode(n.GetID()))
            {
                Nodes.Add(n.GetID(), n);
            }
        }

        public int AddNode(T data)
        {
            return AddNode(data, RootID);
        }

        protected int AddNode(Node<T> node)
        {
            return AddNode(node, RootID);
        }

        public int AddNode(T data, int parentID)
        {

            Node<T> n = new Node<T>(data);

            return AddNode(n, parentID);
        }

        protected int AddNode(Node<T> node, int parentID)
        {
            if (ContainsNode(parentID))
            {
                node.SetParentID(parentID);
                Nodes.Add(node.GetID(), node);
                Nodes[parentID].AddChild(node);
                LastID = node.GetID();
                return LastID;
            }
            node.SetParentID(RootID);
            Nodes.Add(node.GetID(), node);
            Nodes[RootID].AddChild(node);
            LastID = node.GetID();
            return LastID;
        }

        protected bool ContainsNode(int key)
        {
            return Nodes.ContainsKey(key);
        }

        protected void MergeNodeLists(int nodeID, NodeList<T> list)
        {
            if (!Nodes.ContainsKey(nodeID))
            {
                nodeID = RootID;
            }

            GetNode(nodeID).AddChild(list.GetNode(list.RootID));

            foreach (int key in list.GetNodeKeys())
            {
                Nodes.Add(key, list.GetNode(key));
            }
        }

        protected void RemoveNode(int nodeID)
        {
            if (!Nodes.ContainsKey(nodeID))
            {
                return;
            }
            foreach (int key in Nodes[nodeID].GetChildren().Keys)
            {
                RemoveNode(key);
            }
            Nodes.Remove(nodeID);
        }
        /// <summary>
        /// Возвращает ссылку на поддерево
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        protected NodeList<T> GetSubList(int nodeID)
        {
            if (!Nodes.ContainsKey(nodeID))
            {
                return null;
            }

            NodeList<T> result = new NodeList<T>(Nodes[nodeID]);

            SubListsReference(ref result, nodeID);

            return result;
        }

        private void SubListsReference(ref NodeList<T> list, int nodeID)
        {
            foreach (int key in Nodes[nodeID].GetChildren().Keys)
            {
                list.AddNodeDirect(Nodes[key]);
                SubListsReference(ref list, key);
            }
        }
        /// <summary>
        /// Возвращает копию поддерева
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        protected NodeList<T> GetSubListCopy(int nodeID)
        {
            if (!Nodes.ContainsKey(nodeID))
            {
                return null;
            }
            NodeList<T> result = new NodeList<T>(Nodes[nodeID].GetData().Copy());
            SubListsCopy(ref result, nodeID, result.RootID);
            return result;
        }

        protected void SubListsCopy(ref NodeList<T> list, int nodeID, int newParentID)
        {
            foreach (int key in Nodes[nodeID].GetChildrenIDs())
            {
                Node<T> newNode = Nodes[key].Copy();

                list.GetNode(newParentID).AddChild(newNode);

                list.AddNodeDirect(newNode);

                SubListsCopy(ref list, key, newNode.GetID());
            }
        }

        protected void ProcessNodes(INodeProcess<T, string> process)
        {
            int level = 0;

            ProcessNodes(level, Nodes[RootID], process);

            GC.Collect();
        }

        protected void ProcessNodes<resultType>(INodeProcess<T, resultType> process)
        {
            int level = 0;

            ProcessNodes(level, Nodes[RootID], process);

            GC.Collect();
        }


        private void ProcessNodes<resultType>(int level, Node<T> node, INodeProcess<T, resultType> process)
        {
            process.OnStart(level, node);

            if (node.GetChildren().Count == 0)
            {
                process.OnEnd(level, node);
                return;
            }

            foreach (int nodeKey in node.GetChildren().Keys)
            {
                ProcessNodes(level + 1, Nodes[nodeKey], process);
            }

            process.OnEnd(level, node);

        }

        public void Dispose()
        {
            foreach (int key in Nodes.Keys)
            {
                Nodes[key].Dispose();
            }

            Nodes.Clear();

            Nodes = null;

            GC.SuppressFinalize(this);
        }



        private NodeList(Node<T> n)
        {
            Nodes = new Dictionary<int, Node<T>>();

            RootID = n.GetID();

            Nodes.Add(n.GetID(), n);

            LastID = RootID;
        }

        protected NodeList()
        {
            LastID = -1;
            RootID = -1;
            Nodes = new Dictionary<int, Node<T>>();
        }

        public NodeList(T data)
        {
            Nodes = new Dictionary<int, Node<T>>();

            Node<T> n = new Node<T>(data);

            RootID = n.GetID();

            LastID = AddNode(n, RootID);
        }

    }
}

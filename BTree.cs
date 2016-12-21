using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLParserDB
{
    public class BTree
    {
        public class Node
        {
            public List<InterNalNode> InternalNodes { get; set; }
            public Node ParentNode { get; set; }
            public Node()
            {
                this.InternalNodes = new List<InterNalNode>();
                this.ParentNode = null;
            }
            public bool IsFull()
            {

                return InternalNodes.Count == 3;

            }
            public void Sort()
            {
                this.InternalNodes = this.InternalNodes.OrderBy(x => x.Value).ToList();
            }
            public bool IsLeafNode()
            {
                return !this.InternalNodes.Any(x => x.LeftNode != null || x.RightNode != null);
            }
            public bool IsRootNode()
            {
                return this.ParentNode == null;
            }
        }
        public class InterNalNode
        {
            public Node LeftNode { get; set; }
            public Node RightNode { get; set; }
            public string RecordKey { get; set; }
            public int Value { get; set; }


        }

        public List<Node> Nodes { get; set; }
        public List<Node> LeafNodes { get; set; }
        public Node RootNode { get; set; }

        public BTree()
        {
            Nodes = new List<Node>();
            LeafNodes = new List<Node>();
        }
        public Node Search(int value)
        {
            return TreeSearch(this.RootNode, value);
        }
        public Node TreeSearch(Node node, int value)
        {
            if (node.IsLeafNode())
                return node;
            else if (node.InternalNodes.First().Value > value)
            {
                node = GoLeft(node.InternalNodes.First());
                return TreeSearch(node, value);
            }
            else if (node.InternalNodes.First().Value < value && node.InternalNodes[1].Value > value)
            {
                node = GoRight(node.InternalNodes.First());
                return TreeSearch(node, value);
            }
            else if (node.InternalNodes[1].Value < value && node.InternalNodes[2].Value > value)
            {
                node = GoLeft(node.InternalNodes[2]);
                return TreeSearch(node, value);
            }
            else
            {
                node = GoRight(node.InternalNodes[2]);
                return TreeSearch(node, value);
            }

        }
        public bool IsEmpty()
        {
            return Nodes.Count == 0;
        }
        public Node GoLeft(InterNalNode node)
        {
            return node.LeftNode;
        }
        public Node GoRight(InterNalNode node)
        {
            return node.RightNode;
        }

        public void Split(Node node, int value)
        {

            Node newLeafLeft = new Node();
            Node newLeafRight = new Node();
            Node tempNode = new Node();
            tempNode.InternalNodes = new List<InterNalNode>(node.InternalNodes);
            tempNode.InternalNodes.Add(new InterNalNode() { Value = value });
            tempNode.Sort();
            int middleValue = tempNode.InternalNodes[1].Value;
            newLeafLeft.InternalNodes.Add(tempNode.InternalNodes[0]);
            newLeafLeft.InternalNodes.Add(tempNode.InternalNodes[1]);
            newLeafRight.InternalNodes.Add(tempNode.InternalNodes[2]);
            newLeafRight.InternalNodes.Add(tempNode.InternalNodes[3]);
            tempNode.InternalNodes[1].LeftNode = newLeafLeft;
            tempNode.InternalNodes[1].RightNode = newLeafRight;

            if (!node.IsRootNode())
            {
                if (!node.ParentNode.IsFull())
                    node.ParentNode.InternalNodes.Add(tempNode.InternalNodes[1]);
                else
                {

                }
            }
            else
            {
                //Root Node
                if (node.IsFull())
                {
                    Node newRoot = new Node();
                    newRoot.InternalNodes.Add(tempNode.InternalNodes[1]);
                    this.RootNode = newRoot;
                    newLeafLeft.ParentNode = newRoot;
                    newLeafRight.ParentNode = newRoot;
                    this.Nodes.Add(newRoot);
                    this.Nodes.Add(newLeafLeft);
                    this.Nodes.Add(newLeafRight);
                    this.Nodes.Remove(node);
                    //Split(node, tempNode.InternalNodes[1].Value);
                }
                else
                {
                    node.InternalNodes.Clear();
                    node.InternalNodes.Add(tempNode.InternalNodes[1]);
                }
            }


        }

        public void Insert(int value, string recordKey)
        {
            InterNalNode newInternalNode = new InterNalNode()
            {
                RecordKey = recordKey,
                Value = value,
                LeftNode = null,
                RightNode = null
            };
            if (IsEmpty())
            {
                Node newNode = new Node();
                newNode.InternalNodes.Add(newInternalNode);
                this.RootNode = newNode;
                this.Nodes.Add(newNode);
            }
            else
            {
                //bool IsInserted = false;
                Node target = Search(value);
                if (!target.IsFull())
                {
                    target.InternalNodes.Add(newInternalNode);
                    target.Sort();
                }
                else
                {
                    Split(target, value);
                }

            }
        }
        public void Print()
        {
            foreach (var item in this.Nodes)
            {
                item.InternalNodes.ForEach(x => Console.Write(x.Value + ","));
                Console.WriteLine();
            }
        }


    }
}

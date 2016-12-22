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
            public bool IsRootNode(Node root)
            {
                return this == root ;
            }
        }
        public class InterNalNode
        {
            public InterNalNode(int value, string key)
            {
                // TODO: Complete member initialization
                this.Value = value;
                this.RecordKey = key;
                this.LeftNode = null;
                this.RightNode = null;
            }
            public InterNalNode()
            {
                this.Value = 0;
                this.RecordKey = "";
                this.LeftNode = null;
                this.RightNode = null;
            }
            public InterNalNode(InterNalNode node)
            {
                this.Value = node.Value;
                this.RecordKey = node.RecordKey;
                this.LeftNode = node.LeftNode;
                this.RightNode = node.RightNode;
            }
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
            else if (node.InternalNodes[0].Value > value)
            {
                //less than first item
                node = GoLeft(node.InternalNodes.First());
                return TreeSearch(node, value);
            }
            else
            {
                //more than first item
                if (node.InternalNodes.Count >= 2)
                {
                    if (node.InternalNodes[1].Value < value)
                    {//more than second item and more than first item 
                        node = GoRight(node.InternalNodes[1]);
                        return TreeSearch(node, value);
                    }
                    else if(node.InternalNodes[1].Value > value)
                    {
                        if (node.InternalNodes.Count == 3)
                        {
                            if (node.InternalNodes[2].Value > value)
                            {
                                //more than second item but less than third item
                                node = GoLeft(node.InternalNodes[2]);
                                return TreeSearch(node, value);
                            }
                            else
                            {
                                // more than every item
                                node = GoRight(node.InternalNodes[2]);
                                return TreeSearch(node, value);
                            }
                        }
                        else
                        {
                            //less than second item and more than first item
                            node = GoLeft(node.InternalNodes[1]);
                            return TreeSearch(node, value);
                        }
                    }
                    else
                    {
                        return TreeSearch(node, value);
                    }           
                
                }
                else
                {
                    //more than first item but has one item
                    node = GoRight(node.InternalNodes.First());
                    return TreeSearch(node, value);

                }
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
        public int FindMin()
        {
            Node node = this.RootNode;
            while (!node.IsLeafNode())
            {
                node = GoLeft(node.InternalNodes.First());
            }
            return node.InternalNodes.First().Value;
        }
        public int FindMax()
        {
            Node node = this.RootNode;
            while (!node.IsLeafNode())
            {
                node = GoRight(node.InternalNodes.Last());
            }
            return node.InternalNodes.Last().Value;
        }

        public void Split(Node node, int value, string key)
        {
            Node newLeafLeft = new Node();
            Node newLeafRight = new Node();
            Node tempNode = new Node();
            tempNode.InternalNodes = new List<InterNalNode>(node.InternalNodes);
            tempNode.InternalNodes.Add(new InterNalNode() { Value = value, RecordKey = key });
            tempNode.Sort();
            int middleValue = tempNode.InternalNodes[1].Value;
            newLeafLeft.InternalNodes.Add(new InterNalNode(tempNode.InternalNodes[0].Value,tempNode.InternalNodes[0].RecordKey));
            newLeafLeft.InternalNodes.Add(new InterNalNode(tempNode.InternalNodes[1].Value, tempNode.InternalNodes[1].RecordKey));
            newLeafRight.InternalNodes.Add(new InterNalNode(tempNode.InternalNodes[2].Value, tempNode.InternalNodes[2].RecordKey));
            newLeafRight.InternalNodes.Add(new InterNalNode(tempNode.InternalNodes[3].Value, tempNode.InternalNodes[3].RecordKey));
            
            tempNode.InternalNodes[1].LeftNode = newLeafLeft;
            tempNode.InternalNodes[1].RightNode = newLeafRight;

            if (!node.IsRootNode(this.RootNode))
            {
                if (!node.ParentNode.IsFull()){
                    node.ParentNode.InternalNodes.Add(tempNode.InternalNodes[1]);
                    //node.ParentNode.InternalNodes.Add(new InterNalNode(tempNode.InternalNodes[1].Value,tempNode.InternalNodes[1].RecordKey));                    
                    newLeafLeft.ParentNode = node.ParentNode;
                    newLeafRight.ParentNode = node.ParentNode;
                    this.Nodes.Add(newLeafLeft);
                    this.Nodes.Add(newLeafRight);
                    this.Nodes.Remove(node);

                }
                else
                {
                    newLeafLeft.ParentNode = node.ParentNode;
                    newLeafRight.ParentNode = node.ParentNode;
                    this.Nodes.Add(newLeafLeft);
                    this.Nodes.Add(newLeafRight);
                    this.Nodes.Remove(node);
                    //Split(node.ParentNode, tempNode.InternalNodes[1].Value, tempNode.InternalNodes[1].RecordKey);
                    SplitRoot(node.ParentNode, tempNode.InternalNodes[1]);
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
                }
                else
                {
                    node.InternalNodes.Clear();
                    node.InternalNodes.Add(tempNode.InternalNodes[1]);
                }
            }


        }
        void SplitRoot(Node root, InterNalNode insertedNode)
        {
            Node newLeafLeft = new Node();
            Node newLeafRight = new Node();
            Node newRoot = new Node();

            root.InternalNodes.Add(insertedNode);
            root.Sort();
            newLeafLeft.InternalNodes.Add(new InterNalNode(root.InternalNodes[0]));
            newLeafLeft.InternalNodes.Add(new InterNalNode(root.InternalNodes[1]));
            newLeafRight.InternalNodes.Add(new InterNalNode(root.InternalNodes[2]));
            newLeafRight.InternalNodes.Add(new InterNalNode(root.InternalNodes[3]));
            newLeafLeft.ParentNode = newRoot;
            newLeafRight.ParentNode = newRoot;

            newRoot.InternalNodes.Add(new InterNalNode(root.InternalNodes[1].Value,root.InternalNodes[1].RecordKey));
            newRoot.InternalNodes[0].LeftNode = newLeafLeft;
            newRoot.InternalNodes[0].RightNode = newLeafRight;

            this.Nodes.Remove(root);
            this.RootNode = newRoot;
            this.Nodes.Add(newRoot);
            this.Nodes.Add(newLeafLeft);
            this.Nodes.Add(newLeafRight);

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
                    Split(target, value,recordKey);
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

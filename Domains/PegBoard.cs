using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericSearch.Domains
{
    public class Jump : iAction
    {
        public Point source, middle, target;

        public Jump(Point _source, Point _middle, Point _target)
        {
            source = _source;
            middle = _middle;
            target = _target;
        }

        public override int GetHashCode()
        {
            string result = "";

            result += "source:" + source.row + "," + source.col;
            result += ", middle:" + middle.row + "," + middle.col;
            result += ", target:" + target.row + "," + target.col;

            return result.GetHashCode();
        }

        public iDomain Act(iDomain board)
        {
            Node sourceNode = ((PegBoard)board).getNodeAtPoint(source);
            Node middleNode = ((PegBoard)board).getNodeAtPoint(middle);
            Node targetNode = ((PegBoard)board).getNodeAtPoint(target);
            if ((sourceNode == null || middleNode == null || targetNode == null) ||
                (!sourceNode.occupied || !middleNode.occupied || targetNode.occupied))
            {
                return board;
            }
            sourceNode.occupied = false;
            middleNode.occupied = false;
            targetNode.occupied = true;
            return board;
        }

        public override string ToString()
        {
            return source.ToString() + " to " + target.ToString();
        }

        public iAction Clone()
        {
            return new Jump(source, middle, target);
        }
    }

    public class Node
    {
        public bool occupied;
        public Point coord;
        public List<Node> neighbors;

        public Node(Point _coord)
        {
            coord = _coord;
            occupied = false;
            neighbors = new List<Node>();
        }

        public string GetStateHash()
        {
            var result = "";
            result += coord.row + "," + coord.col + ":";
            if (occupied)
            {
                result += "1";
            } else
            {
                result += "0";
            }
            return result;
        }

        public string GetID()
        {
            return coord.ToString();
        }

        public override string ToString()
        {
            var result = GetID() + " - ";
            if (occupied)
            {
                result += "1";
            } else
            {
                result += "0";
            }
            return result;
        }

        public List<Jump> possibleJumps()
        {
            var result = new List<Jump>();
            if (!occupied)
            {
                return result;
            }
            var mobilityRange = new List<int> { -1, 0, 1 };
            var potentialDirs = new List<Point>();
            foreach (int x in mobilityRange)
            {
                foreach (int y in mobilityRange)
                {
                    if (!(x == 0 && y == 0))
                    {
                        potentialDirs.Add(new Point(x, y));
                    }
                }
            }
            foreach (Node n in neighbors.Where(x => x.occupied))
            {
                foreach (Point p in potentialDirs)
                {
                    if (Point.offset(coord, p, Methods.Add) == n.coord)
                    {
                        foreach (Node n2 in n.neighbors.Where(y => !y.occupied))
                        {
                            if (Point.offset(n.coord, p, Methods.Add) == n2.coord)
                            {
                                result.Add(new Jump(coord, n.coord, n2.coord));
                            }
                        }
                    }
                }
            }
            return result;
        }
    }

    public class PegBoard : iDomain
    {
        public Dictionary<string, Node> nodes;

        public PegBoard()
        {
            nodes = new Dictionary<string, Node>();
            init();
        }

        private void createNodes()
        {
            int depth = 5;
            for (int x = 1; x <= depth; x++)
            {
                for (int y = 1; y <= x; y++)
                {
                    var node = new Node(new Point(x, y));
                    node.occupied = true;
                    nodes.Add(node.GetID(), node);
                }
            }
        }

        private List<Point> offsetMatrix()
        {
            var result = new List<Point>();
            result.Add(new Point(0, -1));
            result.Add(new Point(0, 1));
            result.Add(new Point(-1, 0));
            result.Add(new Point(1, 0));
            result.Add(new Point(-1, -1));
            result.Add(new Point(1, 1));
            return result;
        }

        private void LinkNodes()
        {
            foreach (Node n1 in nodes.Values)
            {
                foreach (Node n2 in nodes.Values)
                {
                    foreach (Point p in offsetMatrix())
                    {
                        if (Point.Equals(Point.offset(n1.coord, p), n2.coord)){
                            n1.neighbors.Add(n2);
                        }
                    }
                }
            }
        }

        private void init()
        {
            createNodes();
            LinkNodes();
            getNodeAtPoint(new Point(3, 2)).occupied = false;
        }

        public List<iAction> AvailableActions()
        {
            var result = new List<iAction>();
            foreach (Node n in nodes.Values)
            {
                foreach (Jump j in n.possibleJumps())
                {
                    result.Add(j);
                }
            }
            return result;
        }

        public iDomain Clone()
        {
            var result = new PegBoard();
            foreach (Node n in result.nodes.Values)
            {
                n.occupied = nodes[n.GetID()].occupied;
            }
            return result;
        }

        public string getStateHash()
        {
            var components = new List<String>();
            foreach (Node n in nodes.Values.ToList())
            {
                components.Add(n.GetStateHash());
                components.Add(" | ");
            }
            return components.GetRange(0, components.Count - 1).Merge();

        }

        public bool IsComplete()
        {
            return nodes.Values.Count(x => x.occupied) <= 1;
        }

        public Node getNodeAtPoint(Point p)
        {
            if (nodes.ContainsKey(p.ToString()))
            {
                return nodes[p.ToString()];
            }
            return null;
        }
    }
}

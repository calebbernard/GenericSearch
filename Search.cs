using GenericSearch.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericSearch
{
    enum searchNodeState
    {
        Undiscovered,
        Discovered,
        Exhausted
    }

    public interface iAction
    {
        iDomain Act(iDomain target);
    }

    public interface iDomain
    {
        string getStateHash();
        bool IsComplete();
        iDomain Clone();
        List<iAction> AvailableActions();
    }

    public class SearchResult
    {
        public List<SearchNode> results;
        public int leafNodes;

        public SearchResult()
        {
            results = new List<SearchNode>();
            leafNodes = 0;
        }

        public void AddSolution(SearchNode solution)
        {
            results.Add(solution);
        }
    }

    public abstract class Search
    {
        // okay to run on infinite search spaces
        public abstract string AnalyzeSearchSpace(int maxDepth);

        // Okay to run on infinite search spaces
        public abstract SearchNode AnyPath(iDomain start);

        // Only run on finite search spaces
        public abstract SearchNode ShortestPath();

        // Only run on finite search spaces
        public abstract SearchResult AllPaths(iDomain start);
    }

    public class DepthFirstSearch : Search
    {
        public override SearchResult AllPaths(iDomain start)
        {
            var result = new SearchResult();
            var discovered = new Dictionary<string, SearchNode>();
            var toSearch = new Stack<SearchNode>();
            var rootNode = new SearchNode(start);
            discovered.Add(start.getStateHash(), rootNode);
            toSearch.Push(rootNode);
            while (toSearch.Count() > 0)
            {
                var currentNode = toSearch.Pop();
                if (currentNode.IsComplete())
                {
                    result.AddSolution(currentNode);
                } else
                {
                    var neighbors = currentNode.GetNeighbors();
                    if (neighbors.Count == 0)
                    {
                        result.leafNodes += 1;
                    }
                    foreach (SearchNode neighbor in neighbors)
                    {
                        if (!discovered.ContainsKey(neighbor.getStateHash()))
                        {
                            discovered.Add(neighbor.getStateHash(), neighbor);
                            toSearch.Push(neighbor);
                        }
                    }
                }
            }
            return result;
        }

        public override string AnalyzeSearchSpace(int maxDepth)
        {
            throw new NotImplementedException();
        }

        public override SearchNode AnyPath(iDomain start)
        {
            var discovered = new Dictionary<string, SearchNode>();
            var toSearch = new Stack<SearchNode>();
            var rootNode = new SearchNode(start);
            discovered.Add(start.getStateHash(), rootNode);
            toSearch.Push(rootNode);
            while (toSearch.Count() > 0)
            {
                var currentNode = toSearch.Pop();
                if (currentNode.IsComplete())
                {
                    return currentNode;
                }
                var neighbors = currentNode.GetNeighbors();
                foreach (SearchNode neighbor in neighbors)
                {
                    if (!discovered.ContainsKey(neighbor.getStateHash()))
                    {
                        discovered.Add(neighbor.getStateHash(), neighbor);
                        toSearch.Push(neighbor);
                    }
                }
            }
            return null;
        }

        public override SearchNode ShortestPath()
        {
            throw new NotImplementedException();
        }
    }

    public class BreadthFirstSearch : Search
    {
        public override SearchResult AllPaths(iDomain start)
        {
            var result = new SearchResult();
            var discovered = new Dictionary<string, SearchNode>();
            var toSearch = new Queue<SearchNode>();
            var rootNode = new SearchNode(start);
            discovered.Add(start.getStateHash(), rootNode);
            toSearch.Enqueue(rootNode);
            while (toSearch.Count() > 0)
            {
                var currentNode = toSearch.Dequeue();
                var neighbors = currentNode.GetNeighbors();
                if (neighbors.Count == 0)
                {
                    result.leafNodes += 1;
                }
                foreach (SearchNode neighbor in neighbors)
                {
                    if (neighbor.IsComplete())
                    {
                        result.AddSolution(neighbor);
                        result.leafNodes += 1;
                    } else
                    {
                        if (!discovered.ContainsKey(neighbor.getStateHash()))
                        {
                            discovered.Add(neighbor.getStateHash(), neighbor);
                            toSearch.Enqueue(neighbor);
                        }
                    }
                }
            }
            return result;
        }

        public override string AnalyzeSearchSpace(int maxDepth)
        {
            throw new NotImplementedException();
        }

        public override SearchNode AnyPath(iDomain start)
        {
            var discovered = new Dictionary<string, SearchNode>();
            var toSearch = new Queue<SearchNode>();
            var rootNode = new SearchNode(start);
            discovered.Add(start.getStateHash(), rootNode);
            toSearch.Enqueue(rootNode);
            while (toSearch.Count() > 0)
            {
                var currentNode = toSearch.Dequeue();
                if (currentNode.IsComplete())
                {
                    return currentNode;
                }
                var neighbors = currentNode.GetNeighbors();
                foreach (SearchNode neighbor in neighbors)
                {
                    if (!discovered.ContainsKey(neighbor.getStateHash()))
                    {
                        discovered.Add(neighbor.getStateHash(), neighbor);
                        toSearch.Enqueue(neighbor);
                    } else
                    {
                        if (neighbor.Depth() < discovered[neighbor.getStateHash()].Depth())
                        {
                            discovered[neighbor.getStateHash()].history = neighbor.history;
                        }
                    }
                }
            }
            return null;
        }

        public override SearchNode ShortestPath()
        {
            throw new NotImplementedException();
        }
    }

    // wraps an object with search tags
    public class SearchNode
    {
        public ActionSequence history;
        public iDomain state;

        public SearchNode(iDomain _state, ActionSequence _history = null)
        {
            state = _state;
            if (_history != null)
            {
                history = _history;
            } else
            {
                history = new ActionSequence();
            }
        }

        public List<SearchNode> GetNeighbors()
        {
            var result = new List<SearchNode>();
            foreach (iAction action in state.AvailableActions())
            {
                var vertex = action.Act(state.Clone());
                var vertexHistory = history.Clone();
                vertexHistory.Add(action);
                var newNode = new SearchNode(vertex, vertexHistory);
                result.Add(newNode);
            }
            return result;
        }

        public int Depth()
        {
            return history.Count();
        }

        public string getStateHash()
        {
            return state.getStateHash();
        }

        public bool IsComplete()
        {
            return state.IsComplete();
        }

        public string Path()
        {
            return history.ToString();
        }

        public override string ToString()
        {
            return history.ToString();
        }
    }

    public class ActionSequence
    {
        public List<iAction> history;

        public ActionSequence()
        {
            history = new List<iAction>();
        }

        public ActionSequence Clone()
        {
            var result = new ActionSequence();
            foreach (iAction action in history)
            {
                result.Add(action);
            }
            return result;
        }

        public void Add(iAction action)
        {
            history.Add(action);
        }

        public int Count()
        {
            return history.Count();
        }

        public override string ToString()
        {
            var components = new List<string>();
            foreach (iAction action in history)
            {
                components.Add(action.ToString());
                components.Add("    ");
            }
            return components.GetRange(0, components.Count - 1).Merge();

        }
    }
}

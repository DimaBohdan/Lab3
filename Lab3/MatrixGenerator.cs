using System.Text;
using System.Windows;

namespace Lab3
{
    public class GraphCharacteristic
    {
        private int[] GetDegrees(DirectedGraph graph)
        {
            int[] vector = new int[graph.Vertices];
            int counter = 0;
            for (int i = 0; i < graph.Vertices; i++)
            {
                for (int j = 0; j < graph.Vertices; j++)
                {
                    if (!graph.IsDirected && graph.adjacencyMatrix[j, i] == 1 && j < counter)
                        continue;
                    if (graph.adjacencyMatrix[i, j] == 1)
                    {
                        vector[i]++;
                        vector[j]++;
                    }
                }

                if (!graph.IsDirected)
                    counter++;
            }

            return vector;
        }

        private int[] GetOutDegrees(DirectedGraph graph)
        {
            int[] vector = new int[graph.Vertices];

            for (int i = 0; i < graph.Vertices; i++)
            {
                for (int j = 0; j < graph.Vertices; j++)
                {
                    if (graph.adjacencyMatrix[i, j] == 1)
                        vector[i]++;
                }
            }
            return vector;
        }

        private int[] GetInDegrees(DirectedGraph graph)
        {
            int[] vector = new int[graph.Vertices];

            for (int j = 0; j < graph.Vertices; j++)
            {
                for (int i = 0; i < graph.Vertices; i++)
                {
                    if (graph.adjacencyMatrix[i, j] == 1)
                        vector[j]++;
                }
            }
            return vector;
        }

        private bool IsRegular(DirectedGraph graph)
        {
            int[] degrees = GetDegrees(graph);
            int degree = degrees[0];
            for (int i = 1; i < graph.Vertices; i++)
            {
                if (degrees[i] != degree) return false;
            }
            return true;
        }

        private int[] GetIsolatedVertices(DirectedGraph graph)
        {
            int[] degrees = GetDegrees(graph);
            List<int> result = new List<int>();

            for (int i = 0; i < graph.Vertices; i++)
            {
                if (degrees[i] == 0)
                {
                    result.Add(i);
                }
            }
            return result.ToArray();
        }

        public int[] GetPendantVertices(DirectedGraph graph)
        {
            int[] degrees = GetDegrees(graph);
            List<int> result = new List<int>();

            for (int i = 0; i < graph.Vertices; i++)
            {
                if (degrees[i] == 1)
                {
                    result.Add(i);
                }
            }
            return result.ToArray();
        }

        public int[,] GetReachabilityMatrix(DirectedGraph graph)
        {
            int[,] matrix = (int[,])graph.adjacencyMatrix.Clone();
            int[,] res = MatrixUtils.GetIdentityMatrix(graph.Vertices);
            for (int i = 0; i < graph.Vertices - 1; i++)
            {
                res = MatrixUtils.AddMatrices(res, matrix);
                if (i != graph.Vertices - 2)
                    matrix = MatrixUtils.MultiplyMatrices(matrix, graph.adjacencyMatrix);
            }
            MatrixUtils.GetIdentityMatrix(graph.Vertices);
            return MatrixUtils.ToBoolean(res);
        }

        public int[,] ConnectivityMatrix(DirectedGraph graph)
        {
            int[,] reachabilityMatrix = GetReachabilityMatrix(graph);
            int[,] transposed = MatrixUtils.TransposeMatrix(reachabilityMatrix);
            int[,] connectivityMatrix = MatrixUtils.MultiplyMatricesByElements(reachabilityMatrix, transposed);
            return connectivityMatrix;
        }

        public string GetConnectedComponents(DirectedGraph graph)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Connected Components: ");
            List<List<int>> components = GetStronglyConnectedComponents(graph);
            for (int i = 0; i < components.Count; i++)
            {
                sb.Append($"Component {i + 1}: ");
                sb.Append(string.Join(", ", components[i].Select(v => v + 1)));
                sb.AppendLine(";");
            }
            return sb.ToString();
        }

        public static List<int[]>[] GetDirectPaths(DirectedGraph graph)
        {
            int Size = graph.Vertices;
            List<int[]>[] paths = new List<int[]>[Size];
            for (int i = 0; i < Size; i++)
            {
                paths[i] = new List<int[]>();
                for (int j = 0; j < Size; j++)
                {
                    if (graph.adjacencyMatrix[i, j] == 1)
                    {
                        paths[i].Add(new int[]{ i, j });
                    }
                }
            }
            return paths;
        }

        public static List<int[]>[] FindPathsOfLength(DirectedGraph graph, int pathLength)
        {
            int Size = graph.Vertices;
            int[,] adjacencyPowerMatrix = MatrixUtils.PowerMatrix(graph.adjacencyMatrix, 2);
            List<int[]>[] previousPaths = GetDirectPaths(graph);
            List<int[]>[] currentPaths = new List<int[]>[Size];
            for (int step = 0; step < pathLength - 1; step++)
            {
                for (int source = 0; source < Size; source++)
                {
                    List<int[]> newPaths = new List<int[]>();
                    for (int destination = 0; destination < Size; destination++)
                    {
                        if (adjacencyPowerMatrix[source, destination] > 0)
                        {
                            foreach (int[] currentPath in previousPaths[source])
                            {
                                int lastNode = currentPath[^1];

                                if (graph.adjacencyMatrix[lastNode, destination] == 1)
                                {
                                    var edgeSet = new HashSet<(int, int)>();

                                    for (int i = 0; i < currentPath.Length - 1; i++)
                                        edgeSet.Add((currentPath[i], currentPath[i + 1]));

                                    if (!edgeSet.Contains((lastNode, destination)))
                                    {
                                        int[] extendedPath = new int[currentPath.Length + 1];
                                        Array.Copy(currentPath, extendedPath, currentPath.Length);
                                        extendedPath[^1] = destination;
                                        newPaths.Add(extendedPath);
                                    }
                                }
                            }
                        }
                    }

                    currentPaths[source] = newPaths;
                }

                if (step != pathLength - 2)
                {
                    adjacencyPowerMatrix = MatrixUtils.MultiplyMatrices(adjacencyPowerMatrix, graph.adjacencyMatrix);
                    previousPaths = currentPaths;
                    currentPaths = new List<int[]>[Size];
                }
            }

            return currentPaths;
        }

        private static string PrintPaths(List<int[]>[] paths)
        {
            StringBuilder sb = new StringBuilder();
            for (int source = 0; source < paths.Length; source++)
            {
                if (paths[source] == null || paths[source].Count == 0)
                {
                    continue;
                }    
                foreach (int[] path in paths[source])
                {
                    sb.Append("  " + string.Join(" -> ", path));
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        private static void DFS(DirectedGraph graph, int node, bool[] visited, Stack<int> stack)
        {
            visited[node] = true;
            for (int j = 0; j < graph.Vertices; j++)
            {
                if (graph.adjacencyMatrix[node, j] == 1 && !visited[j])
                {
                    DFS(graph, j, visited, stack);
                }
            }
            stack.Push(node);
        }

        private static void DFSCollectSCC(int[,] transposed, int node, bool[] visited, List<int> scc)
        {
            visited[node] = true;
            scc.Add(node);
            for (int j = 0; j < transposed.GetLength(0); j++)
            {
                if (Convert.ToBoolean(transposed[node, j]) && !visited[j])
                {
                    DFSCollectSCC(transposed, j, visited, scc);
                }
            }
        }

        //public static List<List<int> GetStronglyConnectedComponents(DirectedGraph graph)
        //{
        //    int length = graph.Vertices;
        //    Stack<int> finishStack = new Stack<int>();
        //    bool[] visited = new bool[length];
        //    for (int i = 0; i < length; i++)
        //    {
        //        if (!visited[i])
        //            DFS(graph, i, visited, finishStack);
        //    }
        //    int[,] transposed = MatrixUtils.TransposeMatrix(graph.adjacencyMatrix);

        //    visited = new bool[length];

        //    List<List<int>> sccs = new List<List<int>>();
        //    int[] componentMap = new int[length];

        //    while (finishStack.Count > 0)
        //    {
        //        int node = finishStack.Pop();
        //        if (!visited[node])
        //        {
        //            List<int> scc = new List<int>();
        //            DFSCollectSCC(transposed, node, visited, scc);
        //            scc.Sort();
        //            sccs.Add(scc);
        //        }
        //    }
        //    sccs.Sort((a, b) => a[0].CompareTo(b[0]));
        //    return sccs;
        //}

        public static List<List<int>> GetStronglyConnectedComponents(DirectedGraph graph)
        {
            int length = graph.Vertices;
            bool[,] closure = GetTransitiveClosure(graph.adjacencyMatrix, length);
            List<List<int>> sccs = new List<List<int>>();
            bool[] visited = new bool[length];
            HashSet<int> assignedVertices = new HashSet<int>();

            for (int i = 0; i < length; i++)
            {
                if (visited[i]) continue;

                List<int> scc = new List<int>();
                for (int j = 0; j < length; j++)
                {
                    if (closure[i, j] && closure[j, i])
                    {
                        scc.Add(j);
                        assignedVertices.Add(j);
                    }
                }
                if (scc.Count > 0)
                {
                    foreach (int node in scc)
                    {
                        visited[node] = true;
                    }
                    scc.Sort();
                    sccs.Add(scc);
                }
            }
            for (int i = 0; i < length; i++)
            {
                if (!assignedVertices.Contains(i))
                {
                    sccs.Add(new List<int> { i });
                }
            }
            return sccs.OrderBy(scc => scc[0]).ToList();
        }

        //public static List<List<int>> GetStronglyConnectedComponents(DirectedGraph graph)
        //{
        //    int length = graph.Vertices;
        //    bool[,] closure = GetTransitiveClosure(graph.adjacencyMatrix, length);
        //    List<List<int>> sccs = new List<List<int>>();
        //    bool[] visited = new bool[length];

        //    for (int i = 0; i < length; i++)
        //    {
        //        if (visited[i]) continue;

        //        List<int> scc = new List<int>();
        //        for (int j = 0; j < length; j++)
        //        {
        //            if (closure[i, j] && closure[j, i])
        //            {
        //                scc.Add(j);
        //            }
        //        }
        //        if (scc.Count > 0)
        //        {
        //            foreach (int node in scc)
        //            {
        //                visited[node] = true;
        //            }

        //            scc.Sort();
        //            sccs.Add(scc);
        //        }
        //    }

        //    return sccs.OrderBy(scc => scc[0]).ToList();
        //}

        private static bool[,] GetTransitiveClosure(int[,] adjacencyMatrix, int n)
        {
            bool[,] closure = new bool[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    closure[i, j] = adjacencyMatrix[i, j] == 1;
            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        closure[i, j] |= closure[i, k] && closure[k, j];
            return closure;
        }

        public static int[,] GenerateCondensationMatrix(DirectedGraph graph)
        {
            List<List<int>> sccs = GetStronglyConnectedComponents(graph);
            int n = graph.Vertices;
            List<int>[] adjList = MatrixUtils.ConvertToAdjacencyList(graph.adjacencyMatrix, n);
            Dictionary<int, int> componentMap = new Dictionary<int, int>();
            for (int i = 0; i < sccs.Count; i++)
                foreach (int v in sccs[i])
                    componentMap[v] = i;

            int[,] condensationMatrix = new int[sccs.Count, sccs.Count];
            for (int i = 0; i < n; i++)
            {
                foreach (int j in adjList[i])
                {
                    int u = componentMap[i];
                    int v = componentMap[j];

                    if (u != v)
                        condensationMatrix[u, v] = 1;
                }
            }
            return condensationMatrix;
        }

        public static EdgeLinkedList GetMinSpanningTreePrima(WeightedGraph graph)
        {
            int n = graph.Vertices;
            bool[] inMST = new bool[n];
            EdgeLinkedList skeleton = new EdgeLinkedList();
            PriorityQueue<Edge, int> minHeap = new PriorityQueue<Edge, int>();
            inMST[0] = true;
            AddEdgesToQueue(graph, 0, inMST, minHeap);

            while (skeleton.Count() < n - 1 && minHeap.Count > 0)
            {
                Edge edge = minHeap.Dequeue();

                if (inMST[edge.Vertex2])
                    continue;
                skeleton.AddLast(edge);
                inMST[edge.Vertex2] = true;
                AddEdgesToQueue(graph, edge.Vertex2, inMST, minHeap);
            }

            return skeleton;
        }

        private static void AddEdgesToQueue(WeightedGraph graph, int vertex, bool[] inMST, PriorityQueue<Edge, int> minHeap)
        {
            for (int neighbor = 0; neighbor < graph.Vertices; neighbor++)
            {
                if (!inMST[neighbor] && graph.WeightedMatrix[vertex, neighbor] > 0)
                {
                    Edge edge = new Edge(vertex, neighbor, graph.WeightedMatrix[vertex, neighbor]);
                    minHeap.Enqueue(edge, edge.Weight);
                }
            }
        }

        public string ShowInfo(DirectedGraph graph)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(MatrixUtils.FormatMatrix("Adjacency Matrix:", graph.adjacencyMatrix));
            sb.Append("Degrees: ");
            int[] degrees = GetDegrees(graph);
            for (int i = 0; i < graph.Vertices; i++)
            {
                sb.Append($"deg({i + 1}) = {degrees[i]}; ");
            }
            sb.Append("\r\n");

            if (graph.IsDirected)
            {
                sb.Append("Positive halfdegrees: ");
                int[] posDegrees = GetOutDegrees(graph);
                for (int i = 0; i < graph.Vertices; i++)
                {
                    sb.Append($"deg+({i + 1}) = {posDegrees[i]}; ");
                }
                sb.Append("\r\n");

                sb.Append("Negative halfdegrees: ");
                int[] negDegrees = GetInDegrees(graph);
                for (int i = 0; i < graph.Vertices; i++)
                {
                    sb.Append($"deg-({i + 1}) = {negDegrees[i]}; ");
                }
                sb.Append("\r\n");
            }

            sb.Append("Regularity Degree: ");
            bool regularity = IsRegular(graph);
            int regularDegree = GetInDegrees(graph)[0];
            sb.AppendLine(regularity ? $"{regularDegree}\r\n": "-\r\n");
            sb.Append("Pending vertices: ");
            int[] pendantVertices = GetPendantVertices(graph);
            if (pendantVertices.Length == 0)
            {
                sb.Append("-\r\n");
            }
            else
            {
                for (int i = 0; i < pendantVertices.Length; i++)
                {
                    sb.Append(pendantVertices[i] + 1);
                    sb.Append(i == pendantVertices.Length - 1 ? ";\r\n" : ", ");
                }
            }

            sb.Append("Isolated vertices: ");
            int[] isolatedVertices = GetIsolatedVertices(graph);
            if (isolatedVertices.Length == 0)
            {
                sb.Append("-\r\n");
            }
            else
            {
                for (int i = 0; i < isolatedVertices.Length; i++)
                {
                    sb.Append(isolatedVertices[i] + 1);
                    sb.Append(i == isolatedVertices.Length - 1 ? ";\r\n" : ", ");
                }
            }
            sb.AppendLine("Paths of length 2:");
            sb.Append(PrintPaths(FindPathsOfLength(graph, 2)));
            sb.AppendLine("Paths of length 3:");
            sb.Append(PrintPaths(FindPathsOfLength(graph, 3)));
            int[,] reachabilityMatrix = GetReachabilityMatrix(graph);
            sb.Append(MatrixUtils.FormatMatrix("Reachability Matrix: ", reachabilityMatrix));

            int[,] connectivityMatrix = ConnectivityMatrix(graph);
            sb.Append(MatrixUtils.FormatMatrix("Connectivity Matrix: ", connectivityMatrix));

            sb.Append(GetConnectedComponents(graph));

            int[,] condensationMatrix = GenerateCondensationMatrix(graph);
            sb.Append(MatrixUtils.FormatMatrix("Condensation Matrix: ", condensationMatrix));
            if (graph is WeightedGraph)
            {
                WeightedGraph weightedGraph = graph as WeightedGraph;
                sb.Append(MatrixUtils.FormatMatrix("Weighted Matrix:", weightedGraph.WeightedMatrix));
                EdgeLinkedList minSpanningTree = GetMinSpanningTreePrima(weightedGraph);
                sb.Append("Minimum Spanning Tree: ");
                foreach (Edge edge in minSpanningTree)
                {
                    sb.Append($"({edge.Vertex1 + 1}, {edge.Vertex2 + 1}) ");
                }
                sb.Append($"Minimum Spanning Tree Weight: { minSpanningTree.TotalWeight() }");
                sb.Append("\r\n");
            }
            return sb.ToString();        
        }
    }
}



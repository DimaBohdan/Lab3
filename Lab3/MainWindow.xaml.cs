using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lab3
{
    public partial class MainWindow : Window
    {
        private DirectedGraph graph;
        private WeightedGraph weightedGraph;
        private double[] coefs = Constants.lab3_coefs;
        private bool continueDrawing;

        public MainWindow()
        {
            InitializeComponent();
        }

        private bool ValidateSeedInput(string input, out int seed)
        {
            if (int.TryParse(input, out seed) && seed >= 1000 && seed <= 9999)
            {
                return true;
            }

            seed = 0;
            return false;
        }

        private void OnGenerateDirectedGraphClick(object sender, RoutedEventArgs e)
        {
            GraphCanvas.Children.Clear();
            if (!ValidateSeedInput(SeedInput.Text, out int seed))
            {
                MessageBox.Show("Please enter a valid 4-digit number.");
                return;
            }
            graph = new DirectedGraph(seed, coefs, GraphCanvas.ActualWidth, GraphCanvas.ActualHeight);
            GraphDrawer.DirectedGraphDrawer drawer = new GraphDrawer.DirectedGraphDrawer();
            drawer.DrawGraph(graph, GraphCanvas);
            MatrixWindow matrixWindow = new MatrixWindow(graph);
            matrixWindow.Show();
            int[,] condensationMatrix = GraphCharacteristic.GenerateCondensationMatrix(graph);
            DirectedGraph condensation = new DirectedGraph(condensationMatrix, GraphCanvas.ActualWidth, GraphCanvas.ActualHeight);
            CondensationWindow condensationWindow = new CondensationWindow(condensation);
            condensationWindow.Show();
        }

        private void OnGenerateUndirectedGraphClick(object sender, RoutedEventArgs e)
        {
            GraphCanvas.Children.Clear();
            if (!ValidateSeedInput(SeedInput.Text, out int seed))
            {
                MessageBox.Show("Please enter a valid 4-digit number.");
                return;
            }
            UndirectedGraph graph = new UndirectedGraph(seed, coefs, GraphCanvas.ActualWidth, GraphCanvas.ActualHeight);
            GraphDrawer.UndirectedGraphDrawer drawer = new GraphDrawer.UndirectedGraphDrawer();
            drawer.DrawGraph(graph, GraphCanvas);
            MatrixWindow matrixWindow = new MatrixWindow(graph);
            matrixWindow.Show();
        }

        private void OnGenerateWeightedGraphClick(object sender, RoutedEventArgs e)
        {
            GraphCanvas.Children.Clear();
            if (!ValidateSeedInput(SeedInput.Text, out int seed))
            {
                MessageBox.Show("Please enter a valid 4-digit number.");
                return;
            }
            weightedGraph = new WeightedGraph(seed, coefs, GraphCanvas.ActualWidth, GraphCanvas.ActualHeight);
            GraphDrawer.WeightedGraphDrawer drawer = new GraphDrawer.WeightedGraphDrawer();
            drawer.DrawGraph(weightedGraph, GraphCanvas);
            MatrixWindow matrixWindow = new MatrixWindow(weightedGraph);
            matrixWindow.Show();
        }

        public async Task<(int[,], int[])> BFSearch(DirectedGraph graph, Canvas canvas)
        {
            int n = graph.Vertices;
            int[,] searchMatrix = new int[n, n];
            int[] nodeInfo = new int[n];
            Queue<int> queue = new Queue<int>();
            int k = 0;
            for (int startNode = 0; startNode < n; startNode++)
            {
                double offset = Constants.offset;
                Point[] points = graph.VertexCoordinates;
                if (nodeInfo[startNode] != 0)
                    continue;

                queue.Enqueue(startNode);
                nodeInfo[startNode] = ++k;
                GraphDrawer.DrawVertex(points[startNode], startNode + 1, Brushes.Brown, canvas);
                await WaitForNextStep();

                while (queue.Count > 0)
                {
                    int node = queue.Dequeue();
                    GraphDrawer.DrawVertex(points[node], node + 1, Brushes.OrangeRed, canvas);
                    await WaitForNextStep();

                    for (int neighbor = 0; neighbor < n; neighbor++)
                    {
                        if (graph.adjacencyMatrix[node, neighbor] == 1 && nodeInfo[neighbor] == 0)
                        {
                            queue.Enqueue(neighbor);
                            nodeInfo[neighbor] = ++k;
                            searchMatrix[node, neighbor] = 1;
                            GraphDrawer.DrawDirectedEdge(points[node], points[neighbor], graph.VertexCoordinates, offset, Brushes.Green, canvas);
                            GraphDrawer.DrawVertex(points[neighbor], neighbor + 1, Brushes.Brown, canvas);
                            await WaitForNextStep();
                        }
                    }

                    GraphDrawer.DrawVertex(points[node], node + 1, Brushes.Indigo, canvas);
                    await WaitForNextStep();
                }
            }

            EnsureAllVerticesNumbered(graph, nodeInfo, ref k);
            return (searchMatrix, nodeInfo);
        }

        private static void EnsureAllVerticesNumbered(DirectedGraph graph, int[] nodeInfo, ref int k)
        {
            for (int i = 0; i < graph.Vertices; i++)
            {
                if (nodeInfo[i] == 0)
                {
                    nodeInfo[i] = ++k;
                }
            }
        }

        public async Task<(int[,], int[])> DFSearch(DirectedGraph graph, Canvas canvas)
        {
            int n = graph.Vertices;
            int[,] searchMatrix = new int[n, n];
            int[] nodeInfo = new int[n];
            int k = 0;

            for (int startNode = 0; startNode < n; startNode++)
            {
                if (nodeInfo[startNode] == 0)
                {
                    k = await DepthSearch(graph, startNode, nodeInfo, ++k, searchMatrix, canvas);
                }
            }

            EnsureAllVerticesNumbered(graph, nodeInfo, ref k);
            return (searchMatrix, nodeInfo);
        }

        private async Task<int> DepthSearch(DirectedGraph graph, int node, int[] nodeInfo, int k, int[,] searchMatrix, Canvas canvas)
        {
            nodeInfo[node] = k;
            double offset = Constants.offset;
            Point[] points = graph.VertexCoordinates;
            GraphDrawer.DrawVertex(points[node], node+1, Brushes.Brown, canvas);
            await WaitForNextStep();

            for (int neighbor = 0; neighbor < graph.Vertices; neighbor++)
            {
                if (nodeInfo[neighbor] == 0 && graph.adjacencyMatrix[node, neighbor] == 1)
                {
                    searchMatrix[node, neighbor] = 1;
                    GraphDrawer.DrawDirectedEdge(points[node], points[neighbor], graph.VertexCoordinates, offset, Brushes.Green, canvas);
                    k = await DepthSearch(graph, neighbor, nodeInfo, ++k, searchMatrix, canvas);
                }
            }

            GraphDrawer.DrawVertex(points[node], node+1, Brushes.Indigo, canvas);
            await WaitForNextStep();
            return k;
        }

        private async Task WaitForNextStep()
        {
            continueDrawing = false;
            while (!continueDrawing)
            {
                await Task.Delay(10);
            }
        }

        private void Generate_GraphFirstly()
        {
            if (graph == null)
            {
                MessageBox.Show("Please generate a graph first.");
            }
        }
        private void Generate_WeightedGraphFirstly()
        {
            if (weightedGraph == null)
            {
                MessageBox.Show("Please generate a graph first.");
            }
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.C)
            {
                continueDrawing = true;
            }
            else if (e.Key == Key.B)
            {
                GraphCanvas.Children.Clear();
                Generate_GraphFirstly();
                var (matrix, renumbering) = await BFSearch(graph, GraphCanvas);
                SearchTreeMatrixWindow matrixWindow = new SearchTreeMatrixWindow(matrix, renumbering);
                matrixWindow.Show();
            }
            else if (e.Key == Key.D)
            {
                GraphCanvas.Children.Clear();
                Generate_GraphFirstly();
                var (matrix, renumbering) = await DFSearch(graph, GraphCanvas);
                SearchTreeMatrixWindow matrixWindow = new SearchTreeMatrixWindow(matrix, renumbering);
                matrixWindow.Show();
            }
            else if (e.Key == Key.S)
            {
                GraphCanvas.Children.Clear();
                Generate_WeightedGraphFirstly();

                var edgeList = GraphCharacteristic.GetMinSpanningTreePrima(weightedGraph);
                var node = edgeList.GetFirst();
                var points = weightedGraph.VertexCoordinates;
                double offset = Constants.offset;

                while (node != null)
                {
                    await DrawGraphStep(node.value, points, offset);
                    node = node.next;
                }
            }
        }

        private async Task DrawGraphStep(Edge edge, Point[] points, double offset)
        {
            int start = edge.Vertex1;
            int end = edge.Vertex2;
            GraphDrawer.DrawUndirectedEdge(points[start], points[end], points, offset, Brushes.Green, GraphCanvas);
            GraphDrawer.WeightedGraphDrawer.DrawEdgeWeight(points[start], points[end], weightedGraph.WeightedMatrix[start, end], Brushes.Green, GraphCanvas);
            GraphDrawer.DrawVertex(points[start], start+1, Brushes.Brown, GraphCanvas);
            GraphDrawer.DrawVertex(points[end], end+1, Brushes.Brown, GraphCanvas);
            await WaitForNextStep();
        }
    }
}







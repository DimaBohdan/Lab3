﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lab3
{
    public partial class MainWindow : Window
    {
        private DirectedGraph graph;
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
            WeightedGraph graph = new WeightedGraph(seed, coefs, GraphCanvas.ActualWidth, GraphCanvas.ActualHeight);
            GraphDrawer.WeightedGraphDrawer drawer = new GraphDrawer.WeightedGraphDrawer();
            drawer.DrawGraph(graph, GraphCanvas);
            MatrixWindow matrixWindow = new MatrixWindow(graph);
            matrixWindow.Show();
        }

        public async Task<(int[,], int[])> BFSearch(DirectedGraph graph, Canvas canvas)
        {
            int n = graph.Vertices;
            int[,] searchMatrix = new int[n, n];
            int[] nodeInfo = new int[n];
            Queue<int> queue = new Queue<int>();
            int k = 1;

            for (int startNode = 0; startNode < n; startNode++)
            {
                double offset = Constants.offset;
                Point[] points = graph.VertexCoordinates;
                if (nodeInfo[startNode] != 0)
                    continue;

                queue.Enqueue(startNode);
                nodeInfo[startNode] = ++k;
                GraphDrawer.DrawVertex(points[startNode], startNode+1, Brushes.Brown, canvas);
                await WaitForNextStep();

                while (queue.Count > 0)
                {
                    int node = queue.Dequeue();
                    GraphDrawer.DrawVertex(points[startNode], startNode+1, Brushes.OrangeRed, canvas);
                    await WaitForNextStep();

                    for (int neighbor = 0; neighbor < n; neighbor++)
                    {
                        if (graph.adjacencyMatrix[node, neighbor] == 1 && nodeInfo[neighbor] == 0)
                        {
                            queue.Enqueue(neighbor);
                            nodeInfo[neighbor] = ++k;
                            searchMatrix[node, neighbor] = 1;
                            GraphDrawer.DrawDirectedEdge(points[node], points[neighbor], graph.VertexCoordinates, offset, Brushes.Green, canvas);
                            GraphDrawer.DrawVertex(points[neighbor], neighbor+1, Brushes.Brown, canvas);
                            await WaitForNextStep();
                        }
                    }

                    GraphDrawer.DrawVertex(points[node], node+1, Brushes.Indigo, canvas);
                    await WaitForNextStep();
                }
            }
            SearchRemainingVertices(graph, nodeInfo, k);
            return (searchMatrix, nodeInfo);
        }

        private static void SearchRemainingVertices(DirectedGraph graph, int[] nodeInfo, int k)
        {
            for (int i = 0; i < graph.Vertices; i++)
            {
                if (nodeInfo[i] == 0)
                {
                    k++;
                    nodeInfo[i] = k;
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

            SearchRemainingVertices(graph, nodeInfo, k);
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
                await BFSearch(graph, GraphCanvas);
            }
            else if (e.Key == Key.D)
            {
                GraphCanvas.Children.Clear();
                Generate_GraphFirstly();
                await DFSearch(graph, GraphCanvas);
            }
        }
    }
}







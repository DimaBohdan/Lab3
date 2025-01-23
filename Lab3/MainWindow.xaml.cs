using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GraphAdjacencyMatrix
{
    public partial class MainWindow : Window
    {
        private MatrixGenerator matrixGenerator;
        private GraphDrawer graphDrawer;

        public MainWindow()
        {
            InitializeComponent();
            matrixGenerator = new MatrixGenerator();
            graphDrawer = new GraphDrawer(GraphCanvas);

        }

        private void OnGenerateDirectedGraphClick(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(SeedInput.Text, out int seed) || seed < 1000 || seed > 9999)
            {
                MessageBox.Show("Please enter a valid 4-digit number.");
                return;
            }

            int[,] directedMatrix = matrixGenerator.GenerateDirectedMatrix(seed, out int n);
            DisplayMatrix(directedMatrix, "    Directed Graph");
            GraphCanvas.Children.Clear();
            graphDrawer.DrawDirectedGraph(directedMatrix);
        }

        private void OnGenerateUndirectedGraphClick(object sender, RoutedEventArgs e)
        {
            // Validate seed input
            if (!int.TryParse(SeedInput.Text, out int seed) || seed < 1000 || seed > 9999)
            {
                MessageBox.Show("Please enter a valid 4-digit number.");
                return;
            }

            int[,] directedMatrix = matrixGenerator.GenerateDirectedMatrix(seed, out int n);
            int[,] undirectedMatrix = matrixGenerator.GenerateUndirectedMatrix(directedMatrix);
            DisplayMatrix(undirectedMatrix, "Undirected Graph");
            GraphCanvas.Children.Clear();
            graphDrawer.DrawUndirectedGraph(undirectedMatrix);
        }

        private void DisplayMatrix(int[,] matrix, string graphType)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{graphType} Adjacency Matrix:");
            int n = matrix.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    sb.Append(matrix[i, j] + " ");
                }
                sb.AppendLine();
            }

            MatrixOutput.Text = sb.ToString();
        }
    }
}



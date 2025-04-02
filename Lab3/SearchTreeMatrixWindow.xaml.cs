using System;
using System.Text;
using System.Windows;
using System.Data;

namespace Lab3
{
    public partial class SearchTreeMatrixWindow : Window
    {
        public SearchTreeMatrixWindow(int[,] matrix, int[] renumbering)
        {
            InitializeComponent();
            DisplayAdjacencyMatrix(matrix);
            DisplayVertexRenumbering(renumbering);
        }

        private void DisplayAdjacencyMatrix(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            StringBuilder sb = new StringBuilder();
            sb.Append(MatrixUtils.FormatMatrix("Adjacency Matrix:", matrix));
            AdjacencyMatrixTextBlock.Text = sb.ToString();
        }

        private void DisplayVertexRenumbering(int[] renumbering)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Vertex Renumbering:");
            sb.AppendLine("Old Number -> New Number");

            for (int i = 0; i < renumbering.Length; i++)
            {
                sb.AppendLine($"{i + 1} -> {renumbering[i]}");
            }

            VertexRenumberingTextBlock.Text = sb.ToString();
        }
    }
}

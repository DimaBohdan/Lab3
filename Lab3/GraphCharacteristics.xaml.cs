//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Media.Media3D;
//using System.Windows.Shapes;

//namespace Lab3
//{
//    /// <summary>
//    /// Interaction logic for GraphCharacteristics.xaml
//    /// </summary>
//    public partial class GraphCharacteristics : Window
//    {
//        public GraphCharacteristics(DirectedGraph graph)
//        {
//            InitializeComponent();
//            ShowGraphInfo(graph);
//            ShowAdjacencyMatrix(graph);
//        }
//        private void ShowGraphInfo(DirectedGraph graph)
//        {
//            string info = GraphCharacteristics.ShowInfo(graph);
//            info += "\n" + GraphUtils.ShowWays(graph, 2);
//            info += "\n" + GraphCharacteristics.ShowConnectedComponents(graph);
//            GraphInfoText.Text = info;
//        }

//        private void ShowAdjacencyMatrix(DirectedGraph graph)
//        {
//            int[,] matrix = graph.adjacencyMatrix;
//            int rows = matrix.GetLength(0);
//            int cols = matrix.GetLength(1);

//            DataTable table = new DataTable();
//            for (int j = 0; j < cols; j++)
//                table.Columns.Add($"V{j + 1}", typeof(int));

//            for (int i = 0; i < rows; i++)
//            {
//                var row = table.NewRow();
//                for (int j = 0; j < cols; j++)
//                    row[j] = matrix[i, j];
//                table.Rows.Add(row);
//            }

//            AdjacencyMatrixGrid.ItemsSource = table.DefaultView;
        //}
        //public void DisplayMatrix(int[,] matrix, string title)
        //{
        //    int rows = matrix.GetLength(0);
        //    int cols = matrix.GetLength(1);

        //    MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        //    for (int i = 0; i < rows; i++)
        //        MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        //    for (int j = 0; j < cols; j++)
        //        MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        //    TextBlock titleBlock = new TextBlock
        //    {
        //        Text = title,
        //        FontSize = 18,
        //        FontWeight = FontWeights.Bold,
        //        TextAlignment = TextAlignment.Center,
        //        HorizontalAlignment = HorizontalAlignment.Center,
        //        Padding = new Thickness(5)
        //    };

        //    Grid.SetRow(titleBlock, 0);
        //    Grid.SetColumn(titleBlock, 0);
        //    Grid.SetColumnSpan(titleBlock, cols);
        //    MatrixGrid.Children.Add(titleBlock);

        //    for (int i = 0; i < rows; i++)
        //    {
        //        for (int j = 0; j < cols; j++)
        //        {
        //            TextBlock cell = new TextBlock
        //            {
        //                Text = matrix[i, j].ToString(),
        //                TextAlignment = TextAlignment.Center,
        //                VerticalAlignment = VerticalAlignment.Center,
        //                FontSize = 14,
        //                Padding = new Thickness(5)
        //            };

        //            Grid.SetRow(cell, i + 1);
        //            Grid.SetColumn(cell, j);
        //            MatrixGrid.Children.Add(cell);
        //        }
        //    }
        //}
//    }
//}

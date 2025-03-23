using System.Windows;

namespace Lab3
{
    public partial class MatrixWindow : Window
    {
        GraphCharacteristic graphCharacteristic = new GraphCharacteristic();
        public MatrixWindow(DirectedGraph graph)
        {
            InitializeComponent();
            GraphInfoText.Text = graphCharacteristic.ShowInfo(graph);
        }
    }
}

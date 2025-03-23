using System.Windows;

namespace Lab3
{
    public partial class CondensationWindow : Window
    {
        public CondensationWindow(DirectedGraph condensationGraph)
        {
            InitializeComponent();
            GraphDrawer.DirectedGraphDrawer drawer = new GraphDrawer.DirectedGraphDrawer();
            drawer.DrawGraph(condensationGraph, CondensationCanvas);
        }
    }

}

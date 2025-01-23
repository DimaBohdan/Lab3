using System;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;

public class GraphDrawer
{
    private Canvas canvas;
    private PrimitiveDrawer primitiveDrawer;

    public GraphDrawer(Canvas canvas)
    {
        this.canvas = canvas;
        this.primitiveDrawer = new PrimitiveDrawer();
    }

    public void DrawDirectedGraph(int[,] adjacencyMatrix)
    {
        Point[] positions = CalculateCoordinates(adjacencyMatrix);
        bool[,] processed = new bool[adjacencyMatrix.GetLength(0), adjacencyMatrix.GetLength(1)];

        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
            {
                if (adjacencyMatrix[i, j] == 1)
                {
                    // Проверяем наличие обратной связи
                    if (adjacencyMatrix[j, i] == 1 && !processed[i, j] && !processed[j, i])
                    {
                        // Если есть обратная связь, рисуем антипараллельные дуги
                        primitiveDrawer.DrawAntiparallelArcs(positions[i], positions[j], canvas);

                        // Помечаем обе связи как обработанные
                        processed[i, j] = true;
                        processed[j, i] = true;
                    }
                    else if (!processed[i, j])
                    {
                        // Если обратной связи нет, рисуем стандартную стрелку
                        primitiveDrawer.DrawDirectedEdge(positions[i], positions[j], positions, 15, canvas);
                        processed[i, j] = true;
                    }
                }
            }
        }

        DrawVertices(positions);
    }


    public void DrawUndirectedGraph(int[,] adjacencyMatrix)
    {
        Point[] positions = CalculateCoordinates(adjacencyMatrix);
        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = i; j < adjacencyMatrix.GetLength(1); j++)
            {
                if (adjacencyMatrix[i, j] == 1)
                {
                    primitiveDrawer.DrawUndirectedEdge(positions[i], positions[j], positions, 40, canvas);
                }
            }
        }

        DrawVertices(positions);
    }

    public Point[] CalculateCoordinates(int[,] adjacencyMatrix)
    {
        int n = adjacencyMatrix.GetLength(0);
        double canvasWidth = canvas.ActualWidth;
        double canvasHeight = canvas.ActualHeight;

        Point[] positions = new Point[n];
        double radius = Math.Min(canvasWidth, canvasHeight) / 2 - 20;
        double centerX = canvasWidth / 2;
        double centerY = canvasHeight / 2;
        Point Center = new Point(centerX, centerY);

        for (int i = 0; i < n; i++)
        {
            double angle = 2 * Math.PI * i / n;
            positions[i] = new Point(centerX + radius * Math.Cos(angle), centerY + radius * Math.Sin(angle));
        }
        positions[n - 1] = Center;
        return positions;
    }

    private void DrawVertices(Point[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            primitiveDrawer.DrawVertex(positions[i].X, positions[i].Y, i + 1, canvas);
        }
    }
}




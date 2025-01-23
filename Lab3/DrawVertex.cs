using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

public class PrimitiveDrawer
{
    public void DrawVertex(double X, double Y, int vertexNumber, Canvas canvas)
    {
        if (canvas == null) throw new ArgumentNullException(nameof(canvas));
        Grid vertexContainer = new Grid();
        Ellipse vertex = new Ellipse
        {
            Width = 30,
            Height = 30,
            Fill = Brushes.Blue
        };

        TextBlock label = new TextBlock
        {
            Text = vertexNumber.ToString(),
            Foreground = Brushes.White,
            FontSize = 14,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        vertexContainer.Children.Add(vertex);
        vertexContainer.Children.Add(label);

        Canvas.SetLeft(vertexContainer, X - 15);
        Canvas.SetTop(vertexContainer, Y - 15);

        canvas.Children.Add(vertexContainer);
    }

    public void DrawDirectedEdge(Point start, Point end, Point[] vertexPositions, double minDistance, Canvas canvas)
    {
        if (IsSelfLoop(start, end))
        {
            DrawLoop(start, canvas);
        }
        else if (IsEdgeBlocked(start, end, vertexPositions, minDistance))
        {
            DrawArcWithArrow(start, end, canvas);
        }
        else
        {
            DrawLineWithArrow(start, end, canvas);
        }
    }

    public void DrawUndirectedEdge(Point start, Point end, Point[] vertexPositions, double minDistance, Canvas canvas)
    {
        if (IsSelfLoop(start, end))
        {
            DrawLoop(start, canvas);
        }
        else if (IsEdgeBlocked(start, end, vertexPositions, minDistance))
        {
            DrawArc(start, end, canvas);
        }
        else
        {
            DrawLine(start, end, canvas);
        }
    }

    private bool IsSelfLoop(Point start, Point end) => start.Equals(end);

    private bool IsEdgeBlocked(Point start, Point end, Point[] vertexPositions, double minDistance)
    {
        foreach (Point vertex in vertexPositions)
        {
            if (vertex.Equals(start) || vertex.Equals(end)) continue;

            if (DistanceFromPointToLine(vertex, start, end) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

    private double DistanceFromPointToLine(Point point, Point lineStart, Point lineEnd)
    {
        double a = lineEnd.Y - lineStart.Y;
        double b = lineStart.X - lineEnd.X;
        double c = lineEnd.X * lineStart.Y - lineStart.X * lineEnd.Y;

        return Math.Abs(a * point.X + b * point.Y + c) / Math.Sqrt(a * a + b * b);
    }

    private void DrawLoop(Point vertexPosition, Canvas canvas)
    {
        Ellipse loop = new Ellipse
        {
            Width = 30,
            Height = 30,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };

        Canvas.SetLeft(loop, vertexPosition.X - 15);
        Canvas.SetTop(loop, vertexPosition.Y - 15);

        canvas.Children.Add(loop);
    }

    private void DrawLine(Point start, Point end, Canvas canvas)
    {
        Line edge = new Line
        {
            X1 = start.X,
            Y1 = start.Y,
            X2 = end.X,
            Y2 = end.Y,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };

        canvas.Children.Add(edge);
    }

    private void DrawLineWithArrow(Point start, Point end, Canvas canvas)
    {
        Point adjustedEnd = AdjustPointBeforeVertex(end, start);
        DrawLine(start, end, canvas);
        DrawArrow(start, end, canvas);
    }

    private Point AdjustPointBeforeVertex(Point end, Point start)
    {
        double arrowOffset = 15;
        double dx = end.X - start.X;
        double dy = end.Y - start.Y;
        double length = Math.Sqrt(dx * dx + dy * dy);

        if (length == 0) return end;

        dx /= length;
        dy /= length;

        return new Point(end.X - dx * arrowOffset, end.Y - dy * arrowOffset);
    }

    private void DrawArrow(Point start, Point end, Canvas canvas)
    {
        double arrowSize = 5; // Размер стрелки
        double dx = end.X - start.X;
        double dy = end.Y - start.Y;
        double length = Math.Sqrt(dx * dx + dy * dy);

        if (length == 0) return; // Если начало и конец совпадают, ничего не рисуем

        dx /= length;
        dy /= length;

        Point adjustedEnd = AdjustPointBeforeVertex(end, start); // Смещение конца стрелки
        Point arrowPoint1 = new Point(
            adjustedEnd.X - dx * arrowSize + dy * arrowSize,
            adjustedEnd.Y - dy * arrowSize - dx * arrowSize
        );
        Point arrowPoint2 = new Point(
            adjustedEnd.X - dx * arrowSize - dy * arrowSize,
            adjustedEnd.Y - dy * arrowSize + dx * arrowSize
        );


        Polygon arrowHead = new Polygon
        {
            Points = new PointCollection { adjustedEnd, arrowPoint1, arrowPoint2 },
            Fill = Brushes.Red, // Заливка красным цветом
            Stroke = Brushes.Red, // Рамка стрелки
            StrokeThickness = 1 // Толщина рамки
        };

        canvas.Children.Add(arrowHead);
    }


    private void DrawArc(Point start, Point end, Canvas canvas)
    {
        PathFigure pathFigure = new PathFigure { StartPoint = start };
        Point controlPoint = CalculateArcControlPoint(start, end);
        pathFigure.Segments.Add(new QuadraticBezierSegment(controlPoint, end, true));

        Path arc = new Path
        {
            Data = new PathGeometry(new[] { pathFigure }),
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };

        canvas.Children.Add(arc);
    }

    private void DrawArcWithArrow(Point start, Point end, Canvas canvas)
    {
        DrawArc(start, end, canvas);
        DrawArrow(start, end, canvas);
    }

    private Point CalculateArcControlPoint(Point start, Point end)
    {
        double midX = (start.X + end.X) / 2;
        double midY = (start.Y + end.Y) / 2;

        double offset = 50;
        double dx = end.X - start.X;
        double dy = end.Y - start.Y;
        double length = Math.Sqrt(dx * dx + dy * dy);

        dx /= length;
        dy /= length;

        return new Point(midX - dy * offset, midY + dx * offset);
    }
    public void DrawAntiparallelArcs(Point start, Point end, Canvas canvas)
    {
        double offset = 50; // Смещение дуг для разделения
        DrawArcWithArrow(start, end, canvas);
        DrawArcWithArrow(end, start, canvas);
    }

}


















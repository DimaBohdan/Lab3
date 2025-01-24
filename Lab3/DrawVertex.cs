using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

public class PrimitiveDrawer
{
    public void DrawVertex(double X, double Y, int vertexNumber, Canvas canvas)
    {
        MessageBox.Show($"DrawVertex: {X.ToString()}, {Y.ToString()}");
        if (canvas == null) throw new ArgumentNullException(nameof(canvas));
        Grid vertexContainer = new Grid();
        Ellipse vertex = new Ellipse
        {
            Width = 50,
            Height = 50,
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

        Canvas.SetLeft(vertexContainer, X - 25);
        Canvas.SetTop(vertexContainer, Y - 25);

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

    private bool IsSelfLoop(Point start, Point end)
    {
        return start.Equals(end); // If start equals end, it's a loop.
    }

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

    public Point DrawLoop(Point vertexPosition, Canvas canvas)
    {
        double loopRadius = 25; // Радиус петли
        double canvasCenterX = canvas.ActualWidth / 2;
        double canvasCenterY = canvas.ActualHeight / 2;

        // Смещение центра петли относительно вершины
        double offsetX = (vertexPosition.X < canvasCenterX) ? -loopRadius - 10 : loopRadius + 10;
        double offsetY = (vertexPosition.Y < canvasCenterY) ? -loopRadius - 10 : loopRadius + 10;

        // Если расстояние до центра больше по одной оси, смещаем только по этой оси
        if (Math.Abs(vertexPosition.X - canvasCenterX) > Math.Abs(vertexPosition.Y - canvasCenterY))
            offsetY = 0; // Смещение только по X
        else
            offsetX = 0; // Смещение только по Y

        // Определяем центр петли
        Point loopCenter = new Point(vertexPosition.X + offsetX, vertexPosition.Y + offsetY);

        // Рисуем саму петлю (окружность)
        Ellipse loop = new Ellipse
        {
            Width = loopRadius * 2,
            Height = loopRadius * 2,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };

        // Позиционируем петлю
        Canvas.SetLeft(loop, loopCenter.X - loopRadius);
        Canvas.SetTop(loop, loopCenter.Y - loopRadius);
        canvas.Children.Add(loop);

        return loopCenter; // Возвращаем центр петли
    }

    public void DrawArrowOnLoop(Point vertexPosition, Point loopCenter, double loopRadius, Canvas canvas)
    {
        // Рассчитываем угол для стрелки: выбираем направление дальше от центра вершины
        double dx = loopCenter.X - vertexPosition.X;
        double dy = loopCenter.Y - vertexPosition.Y;
        double angle = Math.Atan2(dy, dx); // Угол в радианах от вершины до центра петли

        // Смещаем угол на 90 градусов, чтобы стрелка была на окружности петли
        angle += Math.PI / 2;

        // Координаты точки на окружности петли
        double arrowX = loopCenter.X + loopRadius * Math.Cos(angle);
        double arrowY = loopCenter.Y + loopRadius * Math.Sin(angle);

        // Конец стрелки (немного за границей петли, для визуального эффекта)
        double arrowTipX = arrowX + 5 * Math.Cos(angle);
        double arrowTipY = arrowY + 5 * Math.Sin(angle);

        // Параметры стрелки
        Point arrowStart = new Point(arrowX, arrowY);
        Point arrowEnd = new Point(arrowTipX, arrowTipY);

        // Рисуем стрелку
        DrawArrow(arrowStart, arrowEnd, canvas);
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
        Point adjustedEnd = AdjustPointBeforeVertex(end, start, 25);
        DrawLine(start, adjustedEnd, canvas);
        DrawArrow(start, adjustedEnd, canvas);
    }
    private Point AdjustPointBeforeVertex(Point end, Point start, double offset)
    {
        double dx = end.X - start.X;
        double dy = end.Y - start.Y;
        double length = Math.Sqrt(dx * dx + dy * dy);

        if (length != 0)
        {
            dx /= length;
            dy /= length;
        }
        return new Point(end.X - dx * offset, end.Y - dy * offset);
    }

    private void DrawArrow(Point start, Point end, Canvas canvas)
    {
        double arrowSize = 5;
        double dx = end.X - start.X;
        double dy = end.Y - start.Y;
        double length = Math.Sqrt(dx * dx + dy * dy);

        if (length != 0)
        {
            dx /= length;
            dy /= length;
        }

        Point arrowPoint1 = new Point(
            end.X - dx * arrowSize + dy * arrowSize,
            end.Y - dy * arrowSize - dx * arrowSize
        );
        Point arrowPoint2 = new Point(
            end.X - dx * arrowSize - dy * arrowSize,
            end.Y - dy * arrowSize + dx * arrowSize
        );

        Polygon arrowHead = new Polygon
        {
            Points = new PointCollection { end, arrowPoint1, arrowPoint2 },
            Fill = Brushes.Red,
            Stroke = Brushes.Red,
            StrokeThickness = 1
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
        Point adjustedEnd = AdjustPointBeforeVertex(end, start, 25);
        DrawArc(start, adjustedEnd, canvas);
        DrawArrow(CalculateArcControlPoint(start, adjustedEnd), adjustedEnd, canvas);
    }

    private Point CalculateArcControlPoint(Point start, Point end)
    {
        double midX = (start.X + end.X) / 2;
        double midY = (start.Y + end.Y) / 2;

        double offset = 60;
        double dx = end.X - start.X;
        double dy = end.Y - start.Y;
        double length = Math.Sqrt(dx * dx + dy * dy);

        if (length != 0)
        {
            dx /= length;
            dy /= length;
        }

        return new Point(midX - dy * offset, midY + dx * offset);
    }

    public void DrawAntiparallelArcs(Point start, Point end, Canvas canvas)
    {

        DrawArcWithArrow(start, end, canvas);
        DrawArcWithArrow(end, start, canvas);
    }
}




















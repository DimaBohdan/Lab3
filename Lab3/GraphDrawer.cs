using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab3
{
    public class GraphDrawer
    {
        public abstract class BaseGraphDrawer<T> where T : DirectedGraph
        {
            public abstract void DrawGraph(T graph, Canvas canvas);
        }

        public class DirectedGraphDrawer : BaseGraphDrawer<DirectedGraph>
        {
            public DirectedGraphDrawer() : base() { }

            public override void DrawGraph(DirectedGraph graph, Canvas canvas)
            {
                Point[] positions = graph.VertexCoordinates;
                int vertices = graph.Vertices;
                bool[,] processed = new bool[vertices, vertices];

                for (int i = 0; i < vertices; i++)
                {
                    for (int j = 0; j < vertices; j++)
                    {
                        if (graph.adjacencyMatrix[i, j] > 0 && !processed[i, j])
                        {
                            if (i == j)
                            {
                                Point loopCenter = DrawLoop(positions[i], canvas);
                                DrawArrowOnLoop(positions[i], loopCenter, Constants.Radius, canvas);
                            }
                            else if (graph.adjacencyMatrix[j, i] > 0 && !processed[j, i])
                            {
                                DrawAntiparallelArcs(positions[i], positions[j], positions, canvas);
                                processed[j, i] = true;
                            }
                            else
                            {
                                DrawDirectedEdge(positions[i], positions[j], positions, Constants.offset, Brushes.Black, canvas);
                            }

                            processed[i, j] = true;
                        }
                    }
                }
                DrawVertices(positions, canvas);
            }
        }

        public class UndirectedGraphDrawer : BaseGraphDrawer<UndirectedGraph>
        {
            public UndirectedGraphDrawer() : base() { }

            public override void DrawGraph(UndirectedGraph graph, Canvas canvas)
            {
                Point[] positions = graph.VertexCoordinates;
                int vertices = graph.Vertices;

                for (int i = 0; i < vertices; i++)
                {
                    for (int j = i; j < vertices; j++)
                    {
                        if (graph.adjacencyMatrix[i, j] > 0)
                        {
                            DrawUndirectedEdge(positions[i], positions[j], positions, Constants.offset, Brushes.Black, canvas);
                        }
                    }
                }
                DrawVertices(positions, canvas);
            }
        }

        public class WeightedGraphDrawer : BaseGraphDrawer<WeightedGraph>
        {
            private static readonly Random _random = new Random();

            public WeightedGraphDrawer() : base() { }

            public override void DrawGraph(WeightedGraph graph, Canvas canvas)
            {
                Point[] positions = graph.VertexCoordinates;
                int vertices = graph.Vertices;

                for (int i = 0; i < vertices; i++)
                {
                    for (int j = 0; j < vertices; j++)
                    {
                        if (graph.adjacencyMatrix[i, j] > 0)
                        {
                            Brush randomColor = GetRandomColor();
                            DrawUndirectedEdge(positions[i], positions[j], positions, Constants.offset, randomColor, canvas);
                            DrawEdgeWeight(positions[i], positions[j], graph.WeightedMatrix[i, j], randomColor, canvas);
                        }
                    }
                }
                DrawVertices(positions, canvas);
            }

            private void DrawEdgeWeight(Point start, Point end, double weight, Brush color, Canvas canvas)
            {
                double midX = (start.X + end.X) / 2;
                double midY = (start.Y + end.Y) / 2;
                double offsetX = (end.Y - start.Y) * 0.001;
                double offsetY = (start.X - end.X) * 0.001;
                Rectangle background = new Rectangle
                {
                    Width = 30,
                    Height = 20,
                    Fill = Brushes.White,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                };
                TextBlock weightText = new TextBlock
                {
                    Text = weight.ToString(),
                    Foreground = color,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                };
                Grid container = new Grid();
                container.Children.Add(background);
                container.Children.Add(weightText);
                Canvas.SetLeft(container, midX + offsetX - background.Width / 2);
                Canvas.SetTop(container, midY + offsetY - background.Height / 2);

                canvas.Children.Add(container);
            }


            private static Brush GetRandomColor()
            {
                return new SolidColorBrush(Color.FromRgb((byte)_random.Next(256), (byte)_random.Next(256), (byte)_random.Next(256)));
            }
        }

        public static void DrawVertices(Point[] positions, Canvas canvas)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                DrawVertex(positions[i], i + 1, Brushes.Blue, canvas);
            }
        }

        public static void DrawVertex(Point position, int vertexNumber, Brush brush, Canvas canvas)
        {
            if (canvas == null) throw new ArgumentNullException(nameof(canvas));
            Grid vertexContainer = new Grid();
            Ellipse vertex = new Ellipse
            {
                Width = Constants.Diameter,
                Height = Constants.Diameter,
                Fill = brush
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

            Canvas.SetLeft(vertexContainer, position.X - Constants.Radius);
            Canvas.SetTop(vertexContainer, position.Y - Constants.Radius);

            canvas.Children.Add(vertexContainer);
        }

        public static void DrawDirectedEdge(Point start, Point end, Point[] vertexPositions, double minDistance, Brush color, Canvas canvas)
        {
            if (IsEdgeBlocked(start, end, vertexPositions, minDistance))
            {
                DrawArcWithArrow(start, end, vertexPositions, canvas);
            }
            else
            {
                DrawLineWithArrow(start, end, color, canvas);
            }
        }

        public static void DrawUndirectedEdge(Point start, Point end, Point[] vertexPositions, double minDistance, Brush brush, Canvas canvas)
        {
            if (IsSelfLoop(start, end))
            {
                DrawLoop(start, canvas);
            }
            else if (IsEdgeBlocked(start, end, vertexPositions, minDistance))
            {
                DrawArc(start, end, brush, vertexPositions, canvas);
            }
            else
            {
                DrawLine(start, end, brush, canvas);
            }
        }

        private static bool IsSelfLoop(Point start, Point end)
        {
            return start.Equals(end);
        }

        private static bool IsEdgeBlocked(Point start, Point end, Point[] vertexPositions, double minDistance)
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

        private static double DistanceFromPointToLine(Point point, Point lineStart, Point lineEnd)
        {
            double a = lineEnd.Y - lineStart.Y;
            double b = lineStart.X - lineEnd.X;
            double c = lineEnd.X * lineStart.Y - lineStart.X * lineEnd.Y;

            return Math.Abs(a * point.X + b * point.Y + c) / Math.Sqrt(a * a + b * b);
        }

        public static Point DrawLoop(Point vertexPosition, Canvas canvas)
        {
            double loopRadius = Constants.Radius;
            double canvasCenterX = canvas.ActualWidth / 2;
            double canvasCenterY = canvas.ActualHeight / 2;
            double offsetX = (vertexPosition.X < canvasCenterX) ? -1.4*loopRadius : 1.4*loopRadius;
            double offsetY = (vertexPosition.Y < canvasCenterY) ? -1.4 * loopRadius : 1.4 * loopRadius;

            if (Math.Abs(vertexPosition.X - canvasCenterX) > Math.Abs(vertexPosition.Y - canvasCenterY))
                offsetY = 0;
            else
                offsetX = 0;

            Point loopCenter = new Point(vertexPosition.X + offsetX, vertexPosition.Y + offsetY);
            Ellipse loop = new Ellipse
            {
                Width = loopRadius * 2,
                Height = loopRadius * 2,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(loop, loopCenter.X - loopRadius);
            Canvas.SetTop(loop, loopCenter.Y - loopRadius);
            canvas.Children.Add(loop);
            return loopCenter;
        }

        public static void DrawArrowOnLoop(Point vertexPosition, Point loopCenter, double loopRadius, Canvas canvas)
        {
            double dx = loopCenter.X - vertexPosition.X;
            double dy = loopCenter.Y - vertexPosition.Y;
            double angle = Math.Atan2(dy, dx);
            angle += Math.PI / 2;
            double arrowX = loopCenter.X + loopRadius * Math.Cos(angle);
            double arrowY = loopCenter.Y + loopRadius * Math.Sin(angle);
            double arrowTipX = arrowX + 5 * Math.Cos(angle);
            double arrowTipY = arrowY + 5 * Math.Sin(angle);
            Point arrowStart = new Point(arrowX, arrowY);
            Point arrowEnd = new Point(arrowTipX, arrowTipY);
            DrawArrow(arrowStart, arrowEnd, canvas);
        }

        private static void DrawLine(Point start, Point end, Brush color, Canvas canvas)
        {
            Line edge = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Stroke = color,
                StrokeThickness = 2
            };

            canvas.Children.Add(edge);
        }

        private static void DrawLineWithArrow(Point start, Point end, Brush color, Canvas canvas)
        {
            Point adjustedEnd = AdjustPointBeforeVertex(end, start, Constants.Radius);
            DrawLine(start, adjustedEnd, color, canvas);
            DrawArrow(start, adjustedEnd, canvas);
        }
        private static Point AdjustPointBeforeVertex(Point end, Point start, double offset)
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

        private static void DrawArrow(Point start, Point end, Canvas canvas)
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

        private static void DrawArc(Point start, Point end, Brush brush, Point[] vertexPositions, Canvas canvas)
        {
            PathFigure pathFigure = new PathFigure { StartPoint = start };
            Point controlPoint = CalculateArcControlPoint(start, end, vertexPositions);
            pathFigure.Segments.Add(new QuadraticBezierSegment(controlPoint, end, true));

            Path arc = new Path
            {
                Data = new PathGeometry(new[] { pathFigure }),
                Stroke = brush,
                StrokeThickness = 2
            };

            canvas.Children.Add(arc);
        }

        private static void DrawArcWithArrow(Point start, Point end, Point[] vertexPositions, Canvas canvas)
        {
            Point adjustedEnd = AdjustPointBeforeVertex(end, start, Constants.Radius);
            DrawArc(start, adjustedEnd, Brushes.Black, vertexPositions, canvas);
            DrawArrow(CalculateArcControlPoint(start, adjustedEnd, vertexPositions), adjustedEnd, canvas);
        }

        private static Point CalculateArcControlPoint(Point start, Point end, Point[] vertexPositions)
        {
            double midX = (start.X + end.X) / 2;
            double midY = (start.Y + end.Y) / 2;
            double controlPointOffset = Constants.offset;

            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length > 0)
            {
                dx /= length;
                dy /= length;
            }

            Point controlPoint = new Point(midX - dy * controlPointOffset, midY + dx * controlPointOffset);
            if (IsEdgeBlocked(start, controlPoint, vertexPositions, controlPointOffset))
            {
                controlPoint = AdjustControlPoint(start, end, controlPoint, vertexPositions, controlPointOffset);
            }
            return controlPoint;
        }

        private static Point AdjustControlPoint(Point start, Point end, Point initialControlPoint, Point[] vertexPositions, double minDistance)
        {
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length > 0)
            {
                dx /= length;
                dy /= length;
            }

            double left = 0, right = 5 * minDistance;
            int maxIterations = 10;
            Point adjustedControlPoint = initialControlPoint;

            for (int i = 0; i < maxIterations; i++)
            {
                double adjustment = (left + right) / 2;
                adjustedControlPoint = new Point(initialControlPoint.X - dy * adjustment, initialControlPoint.Y + dx * adjustment);

                if (IsEdgeBlocked(start, adjustedControlPoint, vertexPositions, minDistance))
                {
                    left = adjustment;
                }
                else
                {
                    right = adjustment;
                }
            }
            return adjustedControlPoint;
        }

        public static void DrawAntiparallelArcs(Point start, Point end, Point[] vertexPositions, Canvas canvas)
        {
            DrawArcWithArrow(start, end, vertexPositions, canvas);
            DrawArcWithArrow(end, start, vertexPositions, canvas);
        }
    }
}





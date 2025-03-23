using System.Windows;
namespace Lab3
{
    public class DirectedGraph
    {
        public int Vertices { get; }
        public Point[] VertexCoordinates { get; }
        public int[,] adjacencyMatrix { get; set; }
        public readonly bool IsDirected;

        public DirectedGraph(int seed, double[] coefs, double width, double height)
        {
            int[] numbers = Utils.GetVariantDigits(seed);
            Vertices = 10 + numbers[3];
            adjacencyMatrix = GetMatrix(Vertices, numbers, coefs);
            VertexCoordinates = GetEdgeCoordinates(width, height);
            IsDirected = SetDirected();
        }

        public DirectedGraph(int[,] matrix, double width, double height)
        {
            adjacencyMatrix = matrix;
            Vertices = matrix.GetLength(0);
            VertexCoordinates = GetEdgeCoordinates(width, height);
            IsDirected = SetDirected();
        }

        protected double[,] GenerateRandomMatrix(double[,] matrix, int variant)
        {
            Random random = new Random(variant);
            for (int i = 0; i < Vertices; i++)
            {
                for (int j = 0; j < Vertices; j++)
                {
                    matrix[i, j] = random.NextDouble() + random.Next(2);
                }
            }
            return matrix;
        }

        protected int[,] MultiplyCoefficientMatrix(double[,] matrix, double coefficient)
        {
            int[,] coefMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < Vertices; i++)
            {
                for (int j = 0; j < Vertices; j++)
                {
                    matrix[i, j] *= coefficient;
                    coefMatrix[i, j] = (int)Math.Floor(matrix[i, j]);
                }
            }
            return coefMatrix;
        }

        protected int[,] GetMatrix(int edges, int[] variantDigits, double[] coefficients)
        {
            double[,] matrix = new double[Vertices, Vertices];
            matrix = GenerateRandomMatrix(matrix, variantDigits[0]);
            double k = 1 - variantDigits[3] * coefficients[0] - variantDigits[4] * coefficients[1] - coefficients[2];
            int[,] coefMatrix = MultiplyCoefficientMatrix(matrix, k);
            return coefMatrix;
        }

        protected Point[] GetEdgeCoordinates(double canvasWidth, double canvasHeight)
        {
            int vertexCount = adjacencyMatrix.GetLength(0);

            Point[] positions = new Point[vertexCount];
            double radius = Math.Min(canvasWidth, canvasHeight) / 2 - 120;
            double centerX = canvasWidth / 2;
            double centerY = canvasHeight / 2;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                double angle = 2 * Math.PI * i / (vertexCount - 1);
                positions[i] = new Point(
                    centerX + radius * Math.Cos(angle),
                    centerY + radius * Math.Sin(angle)
                );
            }

            positions[vertexCount - 1] = new Point(centerX, centerY);
            return positions;
        }

        protected virtual bool SetDirected() => true;

        public virtual EdgeLinkedList ToLinkedList()
        {
            EdgeLinkedList list = new EdgeLinkedList();
            for (int i = 0; i < Vertices; i++)
            {
                for (int j = 0; j < Vertices; j++)
                {
                    if (adjacencyMatrix[i, j] == 1)
                        list.AddFirst(new Edge(i, j));
                }
            }
            return list;
        }
    }

    public class UndirectedGraph : DirectedGraph
    {
        public UndirectedGraph(int seed, double[] coefs, double width, double height) : base(seed, coefs, width, height)
        {
            adjacencyMatrix = GenerateUndirectedMatrix(Vertices, Utils.GetVariantDigits(seed), coefs);
        }
        public UndirectedGraph(int[,] matrix, double width, double height) : base(matrix, width, height) { }
        public int[,] GenerateUndirectedMatrix(int edges, int[] digits, double[] coefficients)
        {
            int[,] undirectedMatrix = base.GetMatrix(edges, digits, coefficients);

            for (int i = 0; i < edges; i++)
            {
                for (int j = 0; j < edges; j++)
                {
                    if (undirectedMatrix[i, j] == 1 || undirectedMatrix[j, i] == 1)
                    {
                        undirectedMatrix[i, j] = undirectedMatrix[j, i] = 1;
                    }
                }
            }
            return undirectedMatrix;
        }
        protected override bool SetDirected() => false;
    }

    public class WeightedGraph : UndirectedGraph
    {
        public int[,] WeightedMatrix { get; }
        public WeightedGraph(int variant, double[] coefs, double width, double height) : base(variant, coefs, width, height)
        {
            WeightedMatrix = GenerateWeightedMatrix(Vertices, variant);
        }

        public WeightedGraph(int[,] adjMatrix, int[,] weightedMatrix, double width, double height) : base(adjMatrix, width, height)
        {
            WeightedMatrix = weightedMatrix;
        }
        protected int[,] GenerateWeightedMatrix(int length, int variant)
        {
            double[,] B = GenerateRandomMatrix(new double[length, length], variant);
            int[,] C = new int[length, length];
            int[,] D = new int[length, length];
            int[,] H = new int[length, length];
            int[,] weighted = new int[length, length];
            int[,] Tr = MatrixUtils.HighTriangleUnitMatrix(length);

            for (int i = 0; i < length; i++)
            {
                for (int j = i; j < length; j++)
                {
                    C[i, j] = (int)Math.Ceiling(B[i, j] * 100 * adjacencyMatrix[i, j]);
                    C[j, i] = C[i, j];
                    D[i, j] = C[i, j] == 0 ? 0 : 1;
                    D[j, i] = D[i, j];
                    int asymmetry = D[i, j] == D[j, i] ? 0 : 1;
                    H[i, j] = asymmetry;
                    H[j, i] = asymmetry;
                    int value = (D[i, j] + H[i, j] * Tr[i, j]) * C[i, j];
                    weighted[i, j] = value;
                    weighted[j, i] = value;
                }
            }
            return weighted;
        }
        public override EdgeLinkedList ToLinkedList()
        {
            EdgeLinkedList list = new EdgeLinkedList();
            for (int i = 0; i < Vertices; i++)
            {
                for (int j = i; j < Vertices; j++)
                {
                    if (adjacencyMatrix[i, j] == 1)
                        list.AddFirst(new Edge(i, j, WeightedMatrix[i, j]));
                }
            }
            return list;
        }
    }
}


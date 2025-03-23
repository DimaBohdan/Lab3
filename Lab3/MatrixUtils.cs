using System.Text;

namespace Lab3
{
    public static class MatrixUtils
    {
        public static int[,] AddMatrices(int[,] first, int[,] second)
        {
            int firstSize = first.GetLength(0);
            int secondSize = first.GetLength(1);
            int[,] result = new int[firstSize, secondSize];
            for (int i = 0; i < firstSize; i++)
            {
                for (int j = 0; j < secondSize; j++)
                {
                    result[i, j] = first[i, j] + second[i, j];
                }
            }
            return result;
        }

        public static int[,] MultiplyMatrices(int[,] first, int[,] second)
        {
            int firstSize = first.GetLength(0);
            int secondSize = second.GetLength(0);
            int midSize = first.GetLength(1);
            int[,] result = new int[firstSize, secondSize];
            for (int i = 0; i < firstSize; i++)
            {
                for (int j = 0; j < secondSize; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < midSize; k++)
                    {
                        result[i, j] += first[i, k] * second[k, j];
                    }
                }
            }
            return result;
        }

        public static int[,] PowerMatrix(int[,] matrix, int pow)
        {
            int[,] result = (int[,])matrix.Clone();
            for (int i = 1; i < pow; i++)
            {
                result = MultiplyMatrices(result, matrix);
            }
            return result;
        }

        public static int[,] MultiplyMatricesByElements(int[,] first, int[,] second)
        {
            int rows = first.GetLength(0);
            int cols = first.GetLength(1);
            int[,] matrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = first[i, j] * second[i, j];
                }
            }
            return matrix;
        }
        public static int[,] ToBoolean(int[,] matrix)
        {
            int firstSize = matrix.GetLength(0);
            int secondSize = matrix.GetLength(1);
            for (int i = 0; i < firstSize; i++)
            {
                for (int j = 0; j < secondSize; j++)
                {
                    matrix[i, j] = matrix[i, j] != 0 ? 1 : 0;
                }
            }
            return matrix;
        }

        public static int[,] TransposeMatrix(int[,] matrix)
        {
            int size = matrix.GetLength(0);
            int[,] transposed = new int[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    transposed[j, i] = matrix[i, j];
                }
            }
            return transposed;
        }

        public static int[,] GetIdentityMatrix(int length)
        {
            int[,] identity = new int[length, length];
            for (int i = 0; i < length; i++)
            {
                identity[i, i] = 1;
            }
            return identity;
        }

        public static List<int>[] ConvertToAdjacencyList(int[,] matrix, int size)
        {
            List<int>[] adjList = new List<int>[size];
            for (int i = 0; i < size; i++)
                adjList[i] = new List<int>();

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (matrix[i, j] > 0)
                        adjList[i].Add(j);

            return adjList;
        }
        public static int[,] HighTriangleUnitMatrix(int size)
        {
            int[,] matrix = new int[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = i + 1; j < size; j++)
                {
                    matrix[i, j] = 1;
                }
            }
            return matrix;
        }

        public static string FormatMatrix(string title, int[,] matrix)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(title);

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sb.Append($"{matrix[i, j], 4}");
                }
                sb.AppendLine();
            }

            sb.AppendLine();
            return sb.ToString();
        }
    }
}

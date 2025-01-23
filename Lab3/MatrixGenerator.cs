using System;

public class MatrixGenerator
{
    public int[,] GenerateDirectedMatrix(int seed, out int n)
    {
        // Extract seed digits
        int n3 = (seed / 10) % 10;
        int n4 = seed % 10;

        // Calculate parameters
        n = n3 + 10;
        double k = 1.0 - n3 * 0.02 - n4 * 0.005 - 0.25;

        // Initialize random generator
        Random random = new Random(seed);

        // Generate matrix
        int[,] adjacencyMatrix = new int[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                double randomValue = random.NextDouble() * 2.0;
                double adjustedValue = randomValue * k;
                adjacencyMatrix[i, j] = adjustedValue >= 1.0 ? 1 : 0;
            }
        }

        return adjacencyMatrix;
    }

    public int[,] GenerateUndirectedMatrix(int[,] directedMatrix)
    {
        int n = directedMatrix.GetLength(0);
        int[,] undirectedMatrix = (int[,])directedMatrix.Clone();

        // Make the matrix symmetric
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (directedMatrix[i, j] == 1 || directedMatrix[j, i] == 1)
                {
                    undirectedMatrix[i, j] = undirectedMatrix[j, i] = 1;
                }
            }
        }

        return undirectedMatrix;
    }
}


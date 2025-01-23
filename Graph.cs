using System;
using System.Collections.Generic;

public class Graph
{
    public int[,] AdjacencyMatrix { get; private set; }
    public int VertexCount { get; private set; }

    public Graph(int n)
    {
        VertexCount = n;
        AdjacencyMatrix = new int[n, n];
    }

    public void GenerateRandomAdjacencyMatrix(bool isDirected)
    {
        Random random = new Random();
        for (int i = 0; i < VertexCount; i++)
        {
            for (int j = (isDirected ? 0 : i); j < VertexCount; j++)
            {
                if (i != j)
                {
                    int value = random.Next(0, 2); // 0 або 1
                    AdjacencyMatrix[i, j] = value;
                    if (!isDirected)
                        AdjacencyMatrix[j, i] = value;
                }
            }
        }
    }
}

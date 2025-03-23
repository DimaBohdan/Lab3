using System;
using System.Collections.Generic;

public class GraphTraversal
{
    private readonly int[,] adjacencyMatrix;
    public readonly int size;
    private readonly bool[] visited;
    private readonly Queue<int> queue;
    private readonly int[,] bfsTree;

    public GraphTraversal(int[,] matrix)
    {
        adjacencyMatrix = matrix;
        size = matrix.GetLength(0);
        visited = new bool[size];
        queue = new Queue<int>();
        bfsTree = new int[size, size];
    }

    public int[,] GetBFSTree()
    {
        Array.Clear(visited, 0, visited.Length);
        Array.Clear(bfsTree, 0, bfsTree.Length);
        queue.Clear();

        for (int startVertex = 0; startVertex < size; startVertex++)
        {
            if (!visited[startVertex])
            {
                BFS(startVertex);
            }
        }

        return (int[,])bfsTree.Clone();
    }

    private void BFS(int startVertex)
    {
        visited[startVertex] = true;
        queue.Enqueue(startVertex);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            for (int i = 0; i < size; i++)
            {
                if (adjacencyMatrix[current, i] == 1 && !visited[i])
                {
                    visited[i] = true;
                    queue.Enqueue(i);
                    bfsTree[current, i] = 1;
                    bfsTree[i, current] = 1;
                }
            }
        }
    }

    public int[,] GetBFSTreeMatrix() => (int[,])bfsTree.Clone();
}




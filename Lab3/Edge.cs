namespace Lab3
{
    public struct Edge
    {
        public Edge(int vertex1, int vertex2, int weight = 0)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Weight = weight;
        }
        public int Vertex1 { get; private set; }
        public int Vertex2 { get; private set; }
        public int Weight { get; private set; }
    }
}

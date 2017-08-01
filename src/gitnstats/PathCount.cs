namespace GitNStats
{
    partial class Program
    {
        internal class PathCount
        {
            public string Path { get; }
            public int Count { get; }

            public PathCount(string path, int count)
            {
                Path = path;
                Count = count;
            }
        }
    }
}
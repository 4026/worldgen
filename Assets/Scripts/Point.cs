public struct Point
{
    public int x, y;
    public Point(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public Point[] Neighbours {
        get {
            return new Point[] {
                new Point(x - 1, y),
                new Point(x, y - 1),
                new Point(x + 1, y),
                new Point(x, y + 1)
            };
        }
    }

    public bool IsInBounds(int minX, int minY, int maxX, int maxY)
    {
        return (x >= minX && x < maxX && y >= minY && y < maxY);
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}
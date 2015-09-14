public struct Point
{
    public int x, y;
    public Point(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public Point[] CardinalNeighbours {
        get {
            return new Point[] {
                new Point(x - 1, y),
                new Point(x, y - 1),
                new Point(x + 1, y),
                new Point(x, y + 1)
            };
        }
    }

    public Point[] DiagonalNeighbours
    {
        get
        {
            return new Point[] {
                new Point(x - 1, y - 1),
                new Point(x + 1, y - 1),
                new Point(x + 1, y + 1),
                new Point(x - 1, y + 1)
            };
        }
    }

    public Point[] Neighbours
    {
        get
        {
            Point[] cardinal = CardinalNeighbours;
            Point[] diagonal = DiagonalNeighbours;

            Point[] output = new Point[cardinal.Length + diagonal.Length];
            System.Array.Copy(cardinal, output, cardinal.Length);
            System.Array.Copy(diagonal, 0, output, cardinal.Length, diagonal.Length);

            return output;
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

    public override int GetHashCode()
    {
        unchecked
        { // Ignore integer overflows in this block
            int hash = 17;
            hash = hash * 23 + this.x.GetHashCode();
            hash = hash * 23 + this.y.GetHashCode();
            return hash;
        }
    }
}
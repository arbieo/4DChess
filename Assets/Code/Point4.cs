public struct Point4
{
    public readonly int x;
    public readonly int y;
    public readonly int z;
    public readonly int w;

    public static readonly Point4 NONE = new Point4(-1, -1, -1, -1);

    public Point4(int x, int y, int z, int w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public override bool Equals(object obj) 
    {
        return obj is Point4 && this == (Point4)obj;
    }
    public override int GetHashCode() 
    {
        return x ^ y ^ z ^ w;
    }
    public static bool operator ==(Point4 l, Point4 r) 
    {
        return (l.x == r.x) && (l.y == r.y) && (l.z == r.z) && (l.w == r.w);
    }
    public static bool operator !=(Point4 l, Point4 r) 
    {
        return !(l == r);
    }
    public static Point4 operator +(Point4 l, Point4 r) 
    {
        return new Point4(l.x + r.x, l.y + r.y, l.y + r.y, l.y + r.y);
    }
    public static Point4 operator -(Point4 l, Point4 r) 
    {
        return new Point4(l.x - r.x, l.y - r.y, l.y - r.y, l.y - r.y);
    }
}
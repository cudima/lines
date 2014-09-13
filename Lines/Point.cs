using System;

namespace Lines
{
  public class Point : IEquatable<Point>
  {
    public ushort Row;
    public ushort Col;

    public Point(ushort row, ushort col)
    {
      Row = row;
      Col = col;
    }

    public double DistanceTo(Point p)
    {
      if (null == p)
        throw new ArgumentNullException("cannot compute distance to null");

      // return Math.Sqrt(Math.Pow(this.Row - p.Row, 2) + Math.Pow(this.Col - p.Col, 2));
      return Math.Abs(this.Row - p.Row) + Math.Abs(this.Col - p.Col);
    }

    public bool Equals(Point p)
    {
      if (ReferenceEquals(p, null))
        return false;

      if (ReferenceEquals(p, this))
        return true;

      return Row == p.Row && Col == p.Col;
    }

    public override bool Equals(object obj)
    {
      Point p = obj as Point;
      if (ReferenceEquals(p, null))
        return base.Equals(obj);
      else
        return Equals(p);
    }

    public static bool operator ==(Point p1, Point p2)
    {
      if (ReferenceEquals(p1, p2))
        return true;

      if ((object)p1 == null || (object)p2 == null)
        return false;

      return p1.Equals(p2);
    }

    public override int GetHashCode()
    {
      return Row * Col;
    }

    public static bool operator !=(Point p1, Point p2)
    {
      return !(p1 == p2);
    }

  }
}
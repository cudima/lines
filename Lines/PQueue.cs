using System;

namespace Lines
{

  public class PriorityQueue
  {
    int m_maxLen;
    Tuple<Point, int>[] m_values;

    /*
    Point[] m_values;
    int[] m_kinks;
    */

    int m_last;
    Point m_end;
    int m_kinkWeight;

    public int Length { get; private set; }

    public PriorityQueue(Point end, int maxLen, int kinkWeight)
    {
      m_maxLen = maxLen;
      // m_values = new Point[m_maxLen];
      m_values = new Tuple<Point, int>[m_maxLen];
      m_kinkWeight = kinkWeight;
      m_end = end;
      m_last = 0;
    }

    public void Enqueue(Point p, int kinks)
    {
      if (m_last == m_maxLen)
        throw new System.Exception("too much pushing");

      int cIndex = m_last;
      Tuple<Point, int> tup = Tuple.Create(p, kinks);
      m_values[cIndex] = tup;

      /*
      m_values[cIndex] = p;
      m_kinks[cIndex] = kinks;
      */

      double cDistance = m_end.DistanceTo(p) + kinks * m_kinkWeight;

      while (cIndex > 0)
      {
        int pIndex = (cIndex - 1) / 2;
        double pDistance = m_end.DistanceTo(m_values[pIndex].Item1) + m_values[pIndex].Item2 * m_kinkWeight;

        if (cDistance < pDistance)
        {
          m_values[cIndex] = m_values[pIndex];
          cIndex = pIndex;
          m_values[cIndex] = tup;
        }
        else
          break;
      }

      m_last++;
    }


    public Point Dequeue()
    {
      if (0 == m_last)
        return null;

      Tuple<Point, int> result = m_values[0];
      m_last--;

      Tuple<Point, int> bubble = m_values[m_last];
      m_values[0] = bubble;

      int cIndex = 0;
      double cDistance = m_end.DistanceTo(bubble.Item1) + bubble.Item2 * m_kinkWeight;

      while (true)
      {
        int lIndex = cIndex * 2 + 1;

        if (lIndex >= m_last)
          break;

        double lDistance = m_end.DistanceTo(m_values[lIndex].Item1) + m_values[lIndex].Item2 * m_kinkWeight;

        int minIndex = lIndex;
        double minDistance = lDistance;

        int rIndex = lIndex + 1;
        if (rIndex < m_last)
        {
          double rDistance = m_end.DistanceTo(m_values[rIndex].Item1) + m_values[rIndex].Item2 * m_kinkWeight;
          if (rDistance < lDistance)
          {
            minDistance = rDistance;
            minIndex = rIndex;
          }
        }

        if (minDistance < cDistance)
        {
          m_values[cIndex] = m_values[minIndex];
          m_values[minIndex] = bubble;
          cIndex = minIndex;
        }
        else
          break;
      }

      return result.Item1;
    }

    public bool IsEmpty()
    {
      return m_last == 0;
    }

  }
}

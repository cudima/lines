using System.Collections.Generic;
using System;

namespace Lines {

  public enum GameState
  {
    Fresh,
    Started,
    Finished
  }

  // public delegate void BoardChangeHandler(Point point, ushort color);

  public class Board
  {
    ushort[,] m_board;
    List<Point> m_empty;
    System.Random m_rand;

    int m_rows, m_cols, m_colors;

    // public event BoardChangeHandler BoardChange;

    public Board(int rows, int cols, int colors)
    {
      m_rows = rows;
      m_cols = cols;
      m_colors = colors;
      m_rand = new System.Random();

      Clear();
    }

    public int NumEmpty { get { return m_empty.Count; } }
   

    public Tuple<Point, ushort> PlaceRandom()
    {
      if (NumEmpty <= 0)
        throw new System.Exception();

      int index = m_rand.Next(NumEmpty - 1);
      ushort color = (ushort)m_rand.Next(1, m_colors + 1);
      Point point = m_empty[index];

      m_board[point.Row, point.Col] = color;
      m_empty.RemoveAt(index);

      return new Tuple<Point, ushort>(point, color);
    }

    public void PlaceItem(Point point, ushort color)
    {
      int index = m_empty.IndexOf(point);
      if (index < 0)
        throw new ArgumentException("couldn't place point. spot occupied");

      m_board[point.Row, point.Col] = color;
      m_empty.RemoveAt(index);
    }


    public void ClearItem(Point point)
    {
      if (m_board[point.Row, point.Col] == 0)
        throw new ArgumentException("couldn't clear point, spot empty");

      m_board[point.Row, point.Col] = 0;
      m_empty.Add(point);
    }

    public ushort GetColor(Point p)
    {
      return m_board[p.Row, p.Col];
    }

    public void Clear()
    {
      m_board = new ushort[m_rows, m_cols];
      m_empty = new List<Point>();

      for (ushort i = 0; i < m_rows; i++)
      {
        for (ushort j = 0; j < m_cols; j++)
        {
          m_empty.Add(new Point(i, j));
        }
      }
    }

    delegate void FindPathD();

    public Stack<Point> FindPath(Point start, Point end)
    {
      if (null == start || null == end || start == end)
        return null;

      Point[,] backPointers = new Point[m_rows, m_cols];
      int[,] pathKinks = new int[m_rows, m_cols];

      PriorityQueue pq = new PriorityQueue(end, m_rows * m_cols, 18);

      Point p = start;
      ushort row = 0, col = 0;

      FindPathD update = () => {
        if (m_board[row, col] == 0 && null == backPointers[row, col])
        {
          Point prev = backPointers[p.Row, p.Col];
          int kinks = pathKinks[p.Row, p.Col];
          if (prev.Row != row && prev.Col != col)
            kinks += 1;

          pq.Enqueue(new Point(row, col), kinks);
          backPointers[row, col] = p;
          pathKinks[row, col] = kinks;
        }
      };

      // short circuit the start pointer
      backPointers[p.Row, p.Col] = p;

      do
      {
        row = p.Row;

        // left
        if (p.Col > 0)
        {
          col = (ushort)(p.Col - 1);
          update();
        }

        // right
        if (p.Col < m_cols - 1)
        {
          col = (ushort)(p.Col + 1);
          update();
        }

        col = p.Col;

        // up
        if (p.Row > 0)
        {
          row = (ushort)(p.Row - 1);
          update();
        }

        // down
        if (p.Row < m_rows - 1)
        {
          row = (ushort)(p.Row + 1);
          update();
        }

        p = pq.Dequeue();
      } while (null != p && p != end);


      // there is no path
      if (null == p)
        return null;

      Stack<Point> path = new Stack<Point>();
      path.Push(p);
      do
      {
        p = backPointers[p.Row, p.Col];
        path.Push(p);
      } while (p != start);

      return path;
    }

    delegate void CheckAddD(int row, int col, List<Point> q);

    public List<Point> CheckLines(Point start)
    {
      ushort color = m_board[start.Row, start.Col];
      if (0 == color)
        throw new ArgumentException("attempted to CheckLines on empty cell");

      List<Point> horizontal = new List<Point>();
      List<Point> vertical = new List<Point>();
      List<Point> leftDiag = new List<Point>();
      List<Point> rightDiag = new List<Point>();

      Queue<Point> toCheck = new Queue<Point>();

      /**
       * check borders and whether the cell should be added
       */
      CheckAddD checkAndAddToQueue = (row, col, q) =>
      {
        if (row >= 0 && col >= 0 && row < m_rows && col < m_cols && m_board[row, col] == color)
        {
          Point p = new Point((ushort)row, (ushort)col);
          toCheck.Enqueue(p);
          q.Add(p);
        }
      };

      // top
      checkAndAddToQueue(start.Row - 1, start.Col - 1, leftDiag);
      checkAndAddToQueue(start.Row - 1, start.Col, vertical);
      checkAndAddToQueue(start.Row - 1, start.Col + 1, rightDiag);

      // middle
      checkAndAddToQueue(start.Row, start.Col - 1, horizontal);
      checkAndAddToQueue(start.Row, start.Col + 1, horizontal);

      // bottom
      checkAndAddToQueue(start.Row + 1, start.Col - 1, rightDiag);
      checkAndAddToQueue(start.Row + 1, start.Col, vertical);
      checkAndAddToQueue(start.Row + 1, start.Col + 1, leftDiag);


      while (toCheck.Count > 0) {
        Point p = toCheck.Dequeue();

        // top
        if (p.Row < start.Row && p.Col < start.Col)
          checkAndAddToQueue(p.Row - 1, p.Col - 1, leftDiag);
        else if (p.Row < start.Row && p.Col == start.Col)
          checkAndAddToQueue(p.Row - 1, p.Col, vertical);
        else if (p.Row < start.Row && p.Col > start.Col)
          checkAndAddToQueue(p.Row - 1, p.Col + 1, rightDiag);

        // middle
        else if (p.Row == start.Row && p.Col < start.Col)
          checkAndAddToQueue(p.Row, p.Col - 1, horizontal);
        else if (p.Row == start.Row && p.Col > start.Col)
          checkAndAddToQueue(p.Row, p.Col + 1, horizontal);

        // bottom
        else if (p.Row > start.Row && p.Col < start.Col)
          checkAndAddToQueue(p.Row + 1, p.Col - 1, rightDiag);
        else if (p.Row > start.Row && p.Col == start.Col)
          checkAndAddToQueue(p.Row + 1, p.Col, vertical);
        else if (p.Row > start.Row && p.Col > start.Col)
          checkAndAddToQueue(p.Row + 1, p.Col + 1, leftDiag);
      }

      List<Point> result = new List<Point>();
      if (leftDiag.Count >= 4)
        result.AddRange(leftDiag);
      if (vertical.Count >= 4)
        result.AddRange(vertical);
      if (rightDiag.Count >= 4)
        result.AddRange(rightDiag);
      if (horizontal.Count >= 4)
        result.AddRange(horizontal);

      if (result.Count > 0)
        result.Add(start);

      return result;
    }

  }

}

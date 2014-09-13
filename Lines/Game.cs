using System;
using System.Collections.Generic;

namespace Lines
{
  public class Game {

    Board m_board;
    View m_view;

    enum GameState
    {
      On,
      Selected,
      Over
    };

    GameState m_state;
    Point m_selectedPoint;
    ushort m_selectedColor;

    void ClickHandler(object sender, Point p)
    {
      // is there a piece on at this point?
      ushort c = m_board.GetColor(p);

      if (null == m_selectedPoint)
      {
        if (0 == c)
          return;

        m_view.Select(p, true);
        m_selectedPoint = p;
        m_selectedColor = c;
      }
      else
      {
        if (0 != c)
        {
          m_view.Select(m_selectedPoint, false);
          m_selectedPoint = p;
          m_selectedColor = c;
          m_view.Select(m_selectedPoint, true);
        }
        else
        {
          Stack<Point> path = m_board.FindPath(m_selectedPoint, p);
          if (null != path) {
            // m_view.Select(m_selectedPoint, false);
            m_view.Move(path.ToArray(), m_selectedColor);

            m_board.ClearItem(m_selectedPoint);
            // m_view.Place(m_selectedPoint, 0);
            m_board.PlaceItem(p, m_selectedColor);
            // m_view.Place(p, m_selectedColor);
            // move(path);
            m_selectedPoint = null;
          }
        }
      }
    }

    void MoveCompletedHandler(Point start, Point end, ushort color)
    {
      List<Point> lines = m_board.CheckLines(end);
      if (lines.Count > 0)
        m_view.Disappear(lines);
      else
        placeNext();
    }


    void placeNext()
    {
      if (m_board.NumEmpty < 3)
      {
        gameOver();
        return;
      }

      Tuple<Point, ushort>[] pieces = new Tuple<Point, ushort>[3];
      List<Point> lines = new List<Point>();

      for (int i = 0; i < 3; i++) {
        pieces[i] = m_board.PlaceRandom();
        m_view.Place(pieces[i].Item1, pieces[i].Item2);
        lines.AddRange(m_board.CheckLines(pieces[i].Item1));
      }

      if (lines.Count > 0)
        m_view.Disappear(lines);
    }

    int checkLines(Point point)
    {
      List<Point> lines = m_board.CheckLines(point);
      if (lines.Count > 0)
      {
        lines.ForEach(delegate(Point p)
        {
          m_view.Place(p, 0);
          m_board.ClearItem(p);
        });
      }

      return lines.Count;
    }

    void gameOver()
    {
      m_state = GameState.Over;
      m_view.OnClick -= ClickHandler;
    }

    public Game(ushort rows, ushort cols, ushort colors)
    {

      m_board = new Board(rows, cols, colors);

      m_view = new View(rows, cols, colors);
      m_view.OnClick += ClickHandler;
      m_view.OnMoveCompleted += MoveCompletedHandler;
      m_view.Show();


      // place first 5
      Tuple<Point, ushort>[] pieces = new Tuple<Point, ushort>[5];

      do
      {
        for (int i = 0; i < 5; i++)
          pieces[i] = m_board.PlaceRandom();

        // if we get a completed line, try again
        if (m_board.CheckLines(pieces[0].Item1).Count > 0)
          m_board.Clear();
        else
          break;
      } while (true);

      for (int i = 0; i < 5; i++)
        m_view.Place(pieces[i].Item1, pieces[i].Item2);

      m_state = GameState.On;
    }


  }

}
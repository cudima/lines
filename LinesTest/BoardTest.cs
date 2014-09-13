using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Lines;

namespace LinesTest
{
  [TestClass]
  public class BoardTest
  {
    [TestMethod]
    public void PlaceRandomTest()
    {
      int rows = 2, cols = 2;
      int colors = 1;
      int cells = rows * cols;

      Board b = new Board(rows, cols, colors);
      Tuple<Point, ushort>[] pieces = new Tuple<Point, ushort>[cells];
      int i, j;

      for (i = 0; i < cells; i++)
      {
        pieces[i] = b.PlaceRandom();
      }

      Assert.AreEqual(b.NumEmpty, 0);
      for (i = 0; i < cells; i++)
      {
        for (j = i + 1; j < cells; j++)
        {
          if (pieces[i].Item1.Equals(pieces[j].Item1))
            Assert.Fail("duplicate empty points");
        }
      }

      PrivateObject b_private = new PrivateObject(b);

      ushort[,] b_m_board = (ushort[,])b_private.GetField("m_board");
      for (i = 0; i < rows; i++)
      {
        for (j = 0; j < cols; j++)
        {
          if (b_m_board[i, j] == 0)
            Assert.Fail("not all cells on the board filled");
        }
      }

      bool threwException = false;
      try
      {
        b.PlaceRandom();
      }
      catch (Exception)
      {
        threwException = true;
      }

      Assert.IsTrue(threwException, "did not throw exception on extra place");
    }

    [TestMethod]
    public void PlaceItemTest()
    {
      int rows = 2, cols = 2;
      int cells = rows * cols;

      ushort i, j;

      bool threwException;

      Board b = new Board(rows, cols, 1);

      for (i = 0; i < rows; i++)
      {
        for (j = 0; j < cols; j++)
        {
          b.PlaceItem(new Point(i, j), 1);
          if (0 == i && 0 == j)
          {
            // test double placement

            threwException = false;
            try
            {
              b.PlaceItem(new Point(i, j), 1);
            }
            catch (ArgumentException)
            {
              threwException = true;
            }

            Assert.IsTrue(threwException, "did not throw exception on double place");
          }
        }
      }

      Assert.AreEqual(b.NumEmpty, 0);

      PrivateObject b_private = new PrivateObject(b);

      ushort[,] b_m_board = (ushort[,])b_private.GetField("m_board");
      for (i = 0; i < rows; i++)
      {
        for (j = 0; j < cols; j++)
        {
          if (b_m_board[i, j] == 0)
            Assert.Fail("not all cells on the board filled");
        }
      }
    }

    [TestMethod]
    public void ClearItemTest()
    {
      Board b = new Board(2, 2, 1);

      b.PlaceItem(new Point(1, 1), 1);
      b.ClearItem(new Point(1, 1));

      Assert.AreEqual(b.NumEmpty, 4, "did not clear item");

      bool threwException = false;
      try
      {
        b.ClearItem(new Point(1, 1));
      }
      catch
      {
        threwException = true;
      }

      Assert.IsTrue(threwException, "did not throw exception on double clear");
    }


    [TestMethod]
    public void FindPathTest()
    {
      /**
       * o o
       * o o
       */

      Board b = new Board(2, 2, 1);
      Point p1 = new Point(0, 0);
      Point p2 = new Point(0, 1);

      Stack<Point> known = new Stack<Point>();
      known.Push(new Point(0, 1));
      known.Push(new Point(0, 0));

      Stack<Point> path = b.FindPath(p1, p2);
      CollectionAssert.AreEqual(known, path);

      /**
       * o o o o
       * o x x o
       * o o x o
       * o o x o
       */
      b = new Board(4, 4, 1);

      b.PlaceItem(new Point(1, 1), 1);
      b.PlaceItem(new Point(1, 2), 1);
      b.PlaceItem(new Point(2, 2), 1);
      b.PlaceItem(new Point(3, 2), 1);

      p1 = new Point(2, 0);
      p2 = new Point(2, 3);

      known = new Stack<Point>();
      known.Push(new Point(2, 3));
      known.Push(new Point(1, 3));
      known.Push(new Point(0, 3));
      known.Push(new Point(0, 2));
      known.Push(new Point(0, 1));
      known.Push(new Point(0, 0));
      known.Push(new Point(1, 0));
      known.Push(new Point(2, 0));

      path = b.FindPath(p1, p2);

      CollectionAssert.AreEqual(known, path);

      path = b.FindPath(p2, p1);
      known = new Stack<Point>(known);

      CollectionAssert.AreEqual(known, path);

      /**
       * o x o o
       * o x x o
       * o o x o
       * o o x o
       */

      b.PlaceItem(new Point(0, 1), 1);

      path = b.FindPath(p1, p2);
      Assert.IsNull(path);

      path = b.FindPath(p1, p1);
      Assert.IsNull(path);
    }

    [TestMethod]
    public void CheckLinesTest()
    {
      Board b = new Board(7, 7, 3);
      Point p, pc;
      List<Point> result;
      List<Point> knownh, knownv, knowndl, knowndr, known;

      pc = new Point(2, 2);

      knownh = new List<Point>();
      ushort row;
      ushort col;
      ushort count;

      /* o o o o o o o o
       * o o o o o o o o
       * + + + + o o o o
       * o o o o o o o o
       * o o o o o o o o
       */
      for (row = pc.Row, col = 0, count = 0; count < 4; col++, count++)
      {
        p = new Point(row, col);
        knownh.Add(p);
        b.PlaceItem(p, 1);
      }

      result = b.CheckLines(knownh.ElementAt(0));
      Assert.AreEqual(0, result.Count);

      /* o o o o o o o o
       * o o o o o o o o
       * + + + + + o o o
       * o o o o o o o o
       * o o o o o o o o
       */
      p = new Point(row, col++);
      knownh.Add(p);
      b.PlaceItem(p, 1);

      // from right
      result = b.CheckLines(knownh.ElementAt(4));
      CollectionAssert.AreEquivalent(knownh, result);
      // from middle
      result = b.CheckLines(knownh.ElementAt(2));
      CollectionAssert.AreEquivalent(knownh, result);
      // from left
      result = b.CheckLines(knownh.ElementAt(0));
      CollectionAssert.AreEquivalent(knownh, result);

      // switch out the color
      b.ClearItem(pc);
      b.PlaceItem(pc, 2);
      // from left
      result = b.CheckLines(knownh.ElementAt(0));
      Assert.AreEqual(0, result.Count);
      // from middle
      result = b.CheckLines(knownh.ElementAt(2));
      Assert.AreEqual(0, result.Count);
      // from right
      result = b.CheckLines(knownh.ElementAt(4));
      Assert.AreEqual(0, result.Count);

      b.ClearItem(pc);
      b.PlaceItem(pc, 1);

      // more than 5
      /* o o o o o o o o
       * o o o o o o o o
       * + + + + + + o o
       * o o o o o o o o
       * o o o o o o o o
       */
      p = new Point(row, col++);
      knownh.Add(p);
      b.PlaceItem(p, 1);
      result = b.CheckLines(p);
      CollectionAssert.AreEquivalent(knownh, result);

      /* o o + o o o o o
       * o o + o o o o o
       * + + + + + + + o
       * o o + o o o o o
       * o o + o o o o o
       */
      knownv = new List<Point>();
      for (row = 0, col = pc.Col, count = 0; count < 5; row++, count++)
      {
        p = new Point(row, col);
        knownv.Add(p);
        if (!p.Equals(pc))
          b.PlaceItem(p, 1);
      }

      // from bottom
      result = b.CheckLines(p);
      CollectionAssert.AreEquivalent(knownv, result);
      // from top
      result = b.CheckLines(knownv.ElementAt(0));
      CollectionAssert.AreEquivalent(knownv, result);

      // two directions
      known = new List<Point>();
      known.AddRange(knownh.AsEnumerable());
      known.AddRange(knownv.AsEnumerable());
      // there is an extra point at the intersection
      known.Remove(pc);
      result = b.CheckLines(pc);
      CollectionAssert.AreEquivalent(known, result);

      /* o o + o o o o o
       * o o + o o o o o
       * + + + + + + + o
       * o o + + o o o o
       * o o + o + o o o
       * o o o o o + o o
       * o o o o o o + o
       */
      knowndl = new List<Point>();
      for (row = pc.Row, col = pc.Col, count = 0; count < 5; row++, col++, count++)
      {
        p = new Point(row, col);
        knowndl.Add(p);
        if (!p.Equals(pc))
          b.PlaceItem(p, 1);
      }

      // from top left (second)
      result = b.CheckLines(knowndl.ElementAt(1));
      CollectionAssert.AreEquivalent(knowndl, result);
      // from bottom right
      result = b.CheckLines(knowndl.ElementAt(4));
      CollectionAssert.AreEquivalent(knowndl, result);

      // three directions
      known.AddRange(knowndl.AsEnumerable());
      known.Remove(pc);
      result = b.CheckLines(pc);
      CollectionAssert.AreEquivalent(known, result);

      /* o o + o + o o o
       * o o + + o o o o
       * + + + + + + + o
       * o + + + o o o o
       * + o + o + o o o
       * o o + o o + o o
       * o o + o o o + o
       */
      knowndr = new List<Point>();
      for (row = 4, col = 0, count = 0; count < 5; row--, col++, count++)
      {
        p = new Point(row, col);
        knowndr.Add(p);
        if (!p.Equals(pc))
          b.PlaceItem(p, 1);
      }

      // from bottom left
      result = b.CheckLines(knowndr.ElementAt(0));
      CollectionAssert.AreEquivalent(knowndr, result);
      // from top right
      result = b.CheckLines(knowndr.ElementAt(4));
      CollectionAssert.AreEquivalent(knowndr, result);

      // four directions
      known.AddRange(knowndr.AsEnumerable());
      known.Remove(pc);
      result = b.CheckLines(pc);
      CollectionAssert.AreEquivalent(known, result);
    }


  }
}

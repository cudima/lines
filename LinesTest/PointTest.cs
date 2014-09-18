using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lines;

namespace LinesTest
{
  [TestClass]
  public class PointTest
  {
    [TestMethod]
    // [System.Diagnostics.CodeAnalysis.SuppressMessage("
    public void EqualityTest()
    {
      Point p1 = new Point(1, 1);
      Point p2 = new Point(2, 2);
      Point p3 = new Point(1, 1);
      
      Assert.AreEqual(p1, p1, "Point not equal to self");
      Assert.AreEqual(p1, p3, "Point not equal to equivalent");
      Assert.AreNotEqual(p1, p2, "Point equal to non-equivalent");
      Assert.AreNotEqual(p1, null, "Point equal to null");

      try
      {
        bool r;
        r = (null == p1);
        r = (null != p1);
        r = (p1 == null);
        r = (p1 != null);
      }
      catch
      {
        Assert.Fail("== operator generates exception on null");
      }

      Assert.IsTrue(p1 == p1);
      Assert.IsTrue(p1 == p3);
      Assert.IsFalse(p1 != p3);
      Assert.IsFalse(p1 == p2);
      Assert.IsTrue(null != p1);
      Assert.IsTrue(p1 != null);
      Assert.IsFalse(p1 == null);
      Assert.IsFalse(null == p1);

      Point p4 = null;
      Assert.IsTrue(null == p4);
      Assert.IsTrue(p4 == null);

    }

    [TestMethod]
    public void DistanceTest()
    {
      Point p1 = new Point(0, 0);
      Point p2 = new Point(1, 1);

      Assert.AreEqual<double>(p1.DistanceTo(p2), 2, "Distance unexpected");
      Assert.AreEqual<double>(p1.DistanceTo(p2), p2.DistanceTo(p1), "Distance not equal in both directions");

      bool threwException = false;

      try
      {
        p1.DistanceTo(null);
      }
      catch (ArgumentNullException)
      {
        threwException = true;
      }

      Assert.IsTrue(threwException, "Did not throw ArgumentNullException");
    }
  }
}

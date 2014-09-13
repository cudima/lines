using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lines;

namespace LinesTest
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class PQueueTest
  {
    public PQueueTest()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    //
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    public void BasicPushPop()
    {
      Point end = new Point(1, 1);
      Point input = new Point(0, 0);
      Point output;

      PriorityQueue pqueue = new PriorityQueue(end, 1);

      pqueue.Enqueue(input);
      output = pqueue.Dequeue();
      Assert.AreSame(input, output);

      output = pqueue.Dequeue();
      Assert.IsNull(output);

      pqueue.Enqueue(input);
      output = pqueue.Dequeue();
      Assert.AreSame(input, output);

      pqueue.Enqueue(end);
      output = pqueue.Dequeue();
      Assert.AreSame(end, output);

      try
      {
        pqueue.Enqueue(input);
        pqueue.Enqueue(input);
      }
      catch (Exception e)
      {
        StringAssert.Contains(e.Message, "pushing");
        return;
      }

      Assert.Fail("No exception thrown");
    }

    [TestMethod]
    public void Ordering()
    {
      Point end = new Point(4, 4);
      Point input1 = new Point(4, 4);
      Point input2 = new Point(3, 3);
      Point input3 = new Point(6, 5);
      Point input4 = new Point(3, 7);

      PriorityQueue pqueue = new PriorityQueue(end, 4);
      pqueue.Enqueue(input4);
      pqueue.Enqueue(input2);
      pqueue.Enqueue(input3);
      pqueue.Enqueue(input1);

      Assert.AreSame(pqueue.Dequeue(), input1);
      Assert.AreSame(pqueue.Dequeue(), input2);
      Assert.AreSame(pqueue.Dequeue(), input3);
      Assert.AreSame(pqueue.Dequeue(), input4);
      Assert.IsNull(pqueue.Dequeue());
    }


  }
}

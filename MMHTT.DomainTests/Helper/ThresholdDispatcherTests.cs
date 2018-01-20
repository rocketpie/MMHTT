using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace MMHTT.Domain.Helper.Tests
{
  /// <summary>
  /// These tests are based on some random. if they fail too often, increase TEST_LENGTH.
  /// </summary>
  [TestClass()]
  public class ThresholdDispatcherTests
  {
    /// <summary>
    /// higher: longer test runs, less failures
    /// </summary>
    static int TEST_LENGTH = 10000;

    [TestMethod()]
    public void DispatchTestFewItems_GetAllDispatched()
    {
      List<int> testData = new List<int>() { 1, 2, 3 };

      var dispatcher = new ThresholdDispatcher<int>(new ConsoleLog(), testData.ToArray(), _ => 1);

      for (int i = 0; i < TEST_LENGTH && testData.Count > 0; i++)
      {
        var result = dispatcher.Dispatch();
        if (testData.Contains(result))
        {
          testData.Remove(result);
        }
      }

      Assert.AreEqual(0, testData.Count);
    }

    [TestMethod()]
    public void DispatchTestOneItem_GetsDispatched()
    {
      List<int> testData = new List<int>() { 1 };

      var dispatcher = new ThresholdDispatcher<int>(new ConsoleLog(), testData.ToArray(), _ => 1);

      for (int i = 0; i < TEST_LENGTH && testData.Count > 0; i++)
      {
        var result = dispatcher.Dispatch();
        if (testData.Contains(result))
        {
          testData.Remove(result);
        }
      }

      Assert.AreEqual(0, testData.Count);
    }



    [TestMethod()]
    public void DispatchTestDifferntWeight_GetDispatchedWeighted()
    {
      var testData = new Dictionary<int, int>();
      testData.Add(1, 1);  // expect this to be dispatched  1/11
      testData.Add(2, 10); // expect this to be dispatched 10/11

      var dispatcher = new ThresholdDispatcher<int>(new ConsoleLog(), testData.Keys.ToArray(), i => testData[i]);

      var result = new Dictionary<int, int>();
      result[1] = 0;
      result[2] = 0;

      for (int i = 0; i < TEST_LENGTH; i++)
      {
        var data = dispatcher.Dispatch();
        result[data]++;
      }

      // *9 is still supposed to work, but 5 is ok, shouldn't fail as often.
      Assert.IsTrue(result[1] * 5 < result[2]);
    }



  }
}
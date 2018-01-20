using System;
using System.Collections.Generic;
using System.Linq;

namespace MMHTT.Domain.Helper
{
  public class ThresholdDispatcher<T>
  {
    private class ThresholdContainer<T>
    {
      public T Obj { get; private set; }
      public int Threshold { get; private set; }

      public ThresholdContainer(T obj, int threshold)
      {
        Obj = obj;
        Threshold = threshold;
      }
    }

    Random _random;
    int _max = 0;
    ThresholdContainer<T>[] _objects;

    public ThresholdDispatcher(T[] objects, Func<T, int> getObjectWeight)
    {
      var list = new List<ThresholdContainer<T>>();

      foreach (var item in objects)
      {
        _max += getObjectWeight(item);
        list.Add(new ThresholdContainer<T>(item, _max));
      }

      _objects = list.ToArray();

      _random = new Random((int)(DateTime.Now.TimeOfDay.TotalMilliseconds % Int32.MaxValue));
    }

    public T Dispatch()
    {
      var i = _random.Next(_max);
      return _objects.First(v => v.Threshold > i).Obj;
    }

  }
}
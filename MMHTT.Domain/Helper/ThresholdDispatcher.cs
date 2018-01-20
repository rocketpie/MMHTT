using System;
using System.Collections.Generic;
using System.Linq;

namespace MMHTT.Domain.Helper
{
  public class ThresholdDispatcher<T>
  {
    private delegate T Dispatcher();
    private class ThresholdContainer<T2>
    {
      public T2 Obj { get; private set; }
      public int Threshold { get; private set; }

      public ThresholdContainer(T2 obj, int threshold)
      {
        Obj = obj;
        Threshold = threshold;
      }
    }

    Random _random;
    Dispatcher _dispatcher;
    int _max = 0;
    ThresholdContainer<T>[] _objects;

    public ThresholdDispatcher(ILog log, T[] objects, Func<T, int> getObjectWeight)
    {
      if (log == null) { throw new ArgumentNullException("log"); }
      if (objects == null) { throw new ArgumentNullException("objects"); }
      if (getObjectWeight == null) { throw new ArgumentNullException("getObjectWeight"); }

      switch (objects.Length)
      {
        case 0:
          log.Warn("no items to dispatch");
          _dispatcher = () => default(T);
          break;

        case 1:
          _dispatcher = () => objects[0];
          break;

        default:
          var list = new List<ThresholdContainer<T>>();
          foreach (var item in objects)
          {
            _max += getObjectWeight(item);
            list.Add(new ThresholdContainer<T>(item, _max));
          }
          _objects = list.ToArray();
          _random = new Random((int)(DateTime.Now.TimeOfDay.TotalMilliseconds % Int32.MaxValue));

          _dispatcher = () =>
          {
            var random = _random.Next(_max);
            return _objects.First(v => v.Threshold > random).Obj;
          };
          break;
      }
    }

    public T Dispatch() => _dispatcher();
  }
}
using System;

namespace Util {
  public class UnityClock : IClock {
    public TimeSpan Now {
      get {
        return TimeSpan.FromSeconds(UnityEngine.Time.time);
      }
    }
  }
}
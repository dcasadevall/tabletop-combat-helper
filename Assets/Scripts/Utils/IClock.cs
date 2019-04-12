using System;

namespace Util {
  public interface IClock {
    /// <summary>
    /// Returns the current time, in <see cref="TimeSpan"/> format.
    /// This is specially helpful for time difference calculations,
    /// as well as abstracting floating point issues.
    /// </summary>
    TimeSpan Now { get; }
  }
}
using System;
using System.Runtime.Serialization;
using CommandSystem;

namespace Replays.Playback {
    /// <summary>
    /// An adapter class used to expose a different execution time of the original snapshot.
    /// This is useful for replays, which may modify the perceived command execution time.
    /// </summary>
    public class ReplaySnapshot {
        public ICommandSnapshot CommandSnapshot { get; }
        public TimeSpan ReplayTime { get; }

        public ReplaySnapshot(ICommandSnapshot commandSnapshot, TimeSpan replayTime) {
            CommandSnapshot = commandSnapshot;
            ReplayTime = replayTime;
        }
    }
}
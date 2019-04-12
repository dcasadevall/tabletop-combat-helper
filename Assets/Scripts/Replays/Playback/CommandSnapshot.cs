using System;
using System.Runtime.Serialization;
using CommandSystem;

namespace Replays.Playback {
    public class CommandSnapshot {
        public readonly ICommand<> command;
        public readonly ISerializable data;
        public readonly TimeSpan timeSpan;
        
        public CommandSnapshot(ISerializable data, TimeSpan timeSpan) {
            this.data = data;
            this.timeSpan = timeSpan;
        }
    }
}
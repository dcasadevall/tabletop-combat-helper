using System;
using System.Runtime.Serialization;
using CommandSystem;

namespace Replays.Playback {
    public class CommandSnapshot {
        public readonly ISerializable data;
        public readonly ICommand<ISerializable> command;
        public readonly TimeSpan timeSpan;
        
        public CommandSnapshot(ICommand<ISerializable> command, ISerializable data, TimeSpan timeSpan) {
            this.command = command;
            this.data = data;
            this.timeSpan = timeSpan;
        }
    }
}
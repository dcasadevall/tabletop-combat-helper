using System;

namespace Replays.Persistence {
    public class CommandHistorySaveInfo {
        public readonly string name;
        
        public CommandHistorySaveInfo(string name) {
            this.name = name;
        }
    }
}
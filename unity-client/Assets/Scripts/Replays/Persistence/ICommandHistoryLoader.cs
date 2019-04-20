using System;
using System.Collections.Generic;

namespace Replays.Persistence {
    public interface ICommandHistoryLoader {
        IEnumerable<string> SavedCommandHistories { get; }
        void LoadCommandHistory(string saveName);
    }
}
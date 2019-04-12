namespace Replays.Playback {
    public interface IReplayPlaybackManager {
        /// <summary>
        /// Called whenever a new command is queued while playback is going on. This should interrupt the current
        /// playback.
        /// </summary>
        event System.Action PlaybackInterrupted;
        
        /// <summary>
        /// Gets the current progress relative to all commands to be played.
        /// From 0 to 1
        /// </summary>
        float Progress { get; }
        /// <summary>
        /// A multiplier at which the game will be replayed.
        /// 1 is the base value, with common values such as 2, 4, 8... being used as well.
        /// By default
        /// </summary>
        uint PlaybackSpeed { get; set; }
        /// <summary>
        /// Enters playback, which acquires input locks to avoid new commands being queued while playback happens.
        /// The replay starts from the very first command.
        ///
        /// If called while Paused, simply resumes playback.
        /// </summary>
        void Play();
        /// <summary>
        /// Enters playback if necessary, acquiring input locks.
        /// Seeks to the given normalized progress (0 to 1), which corresponds to the fraction of the command being
        /// player, over the total amount of commands.
        ///
        /// Note that some commands may be batched between other commands (i.e: painting)
        /// </summary>
        /// <param name="progress"></param>
        void Seek(float progress);
        /// <summary>
        /// Pauses the current playback.
        /// This does not release the input locks, but rather just stops command execution altogether.
        /// </summary>
        void Pause();
        /// <summary>
        /// Stops the current playback, resuming the game at the latest command that was found.
        /// This effectively "fast forwards" the game to the very end of the replay before resuming it.
        /// 
        /// This releases any input locks the playback manager may hold.
        /// </summary>
        void Stop();
        /// <summary>
        /// This effectively turns the current state of the playback into the game state.
        /// It removes any future commands that have not been played yet, and makes the latest command played
        /// our "present".
        /// 
        /// This is useful for loading replays and restoring a game in the middle of such replay, without going
        /// to the very end of it.
        /// </summary>
        void EraseFuture();
    }
}
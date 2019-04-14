using System;
using System.Collections.Generic;
using CommandSystem;
using Util;
using Zenject;

namespace Replays.Playback {
    public class ReplayPlaybackManager : ITickable, IReplayPlaybackManager, ICommandQueueListener {
        /// <summary>
        /// This keeps the replay dynamic. It clips any excessive idle time between commands.
        /// </summary>
        private static TimeSpan kMaxTimeBetweenCommands = TimeSpan.FromSeconds(1);
        
        public event Action PlaybackInterrupted = delegate {};
        
        private readonly ICommandQueue _commandQueue;
        private readonly ICommandFactory _commandFactory;
        private readonly IClock _clock;

        private TimeSpan _currentTime;
        
        private bool _isPlaying;
        public bool IsPlaying {
            get {
                return _isPlaying;
            }
        }

        private bool _isPaused;
        public bool IsPaused {
            get {
                return _isPaused;
            }
        }

        public float Progress {
            get {
                if (TotalTime == TimeSpan.Zero) {
                    return 1;
                }
                
                return (float) (_currentTime.TotalSeconds / TotalTime.TotalSeconds);
            }
        }
        
        public uint PlaybackSpeed { get; set; }

        private TimeSpan TotalTime {
            get {
                if (_futureCommands.Count == 0 && _pastCommands.Count == 0) {
                    return TimeSpan.Zero;
                }

                if (_futureCommands.Count == 0) {
                    return _pastCommands.Last.Value.ReplayTime - _pastCommands.First.Value.ReplayTime;
                }

                if (_pastCommands.Count == 0) {
                    return _futureCommands.Last.Value.ReplayTime - _futureCommands.First.Value.ReplayTime;
                }

                return _futureCommands.Last.Value.ReplayTime - _pastCommands.First.Value.ReplayTime;
            }
        }

        /// <summary>
        /// Queue containing commands that have been executed up to this point
        /// </summary>
        private readonly LinkedList<ReplaySnapshot> _pastCommands = new LinkedList<ReplaySnapshot>();
        
        /// <summary>
        /// Queue containing commands that have not yet been executed, but will be executed as the playback continues.
        /// </summary>
        private readonly LinkedList<ReplaySnapshot> _futureCommands = new LinkedList<ReplaySnapshot>();

        public ReplayPlaybackManager(ICommandQueue commandQueue, IClock clock) {
            _commandQueue = commandQueue;
            _clock = clock;
            
            // We do this here and not as part of IInitializable to avoid race condition issues.
            _commandQueue.AddListener(this);
        }

        public void HandleCommandQueued(ICommandSnapshot commandSnapshot) {
            // Make sure we do not have commands in future before we queue up past commands.
            // This is a safety net.
            // One should explicitly call Stop() or EraseFuture(), before any new command is queued.
            // to be dumped.
            // TODO: Use seek
            while (_futureCommands.Count > 0) {
                RedoNextCommand();

                if (_futureCommands.Count == 0) {
                    PlaybackInterrupted.Invoke();
                }
            }

            TimeSpan timeFromPreviousCommand = GetClippedTimeFromPreviousCommand(commandSnapshot);
            ReplaySnapshot replaySnapshot = new ReplaySnapshot(commandSnapshot, timeFromPreviousCommand);
            _pastCommands.AddLast(replaySnapshot);
        }

        private TimeSpan GetClippedTimeFromPreviousCommand(ICommandSnapshot commandSnapshot) {
            if (_pastCommands.Count == 0) {
                return TimeSpan.Zero;
            }

            if (_pastCommands.Last.Value.CommandSnapshot.Command.IsInitialGameStateCommand) {
                return TimeSpan.Zero;
            }

            TimeSpan previousExecutionTime = _pastCommands.Last.Value.CommandSnapshot.ExecutionTime;
            TimeSpan previousReplayTime = _pastCommands.Last.Value.ReplayTime;
            TimeSpan timeFromPreviousCommand = commandSnapshot.ExecutionTime - 
                                               previousExecutionTime;

            if (timeFromPreviousCommand > kMaxTimeBetweenCommands) {
                return kMaxTimeBetweenCommands + previousReplayTime;
            }

            return timeFromPreviousCommand + previousReplayTime;
        }

        public void Tick() {
            if (!_isPlaying) {
                return;
            }

            if (_isPaused) {
                return;
            }
            
            if (_currentTime == TotalTime) {
                Stop();
            }

            _currentTime += _clock.Delta;
            ReplayCommandsAtCurrentTime();
        }

        /// <summary>
        /// Replays or undoes any commands up to or prior to the current playback time.
        /// Used during Play or Seek
        /// </summary>
        private void ReplayCommandsAtCurrentTime() {
            while (_pastCommands.Count > 0 && _currentTime < _pastCommands.Last.Value.ReplayTime) {
                UndoPreviousCommand();
            }

            while (_futureCommands.Count > 0 && _currentTime > _futureCommands.First.Value.ReplayTime) {
                RedoNextCommand();
            }
        }

        public void Play() {
            // Rewind to the very first frame if we are at the end.
            if (!_isPlaying && (Progress.Equals(0.0f) || Progress.Equals(1.0f))) {
                _currentTime = _clock.Now;
                Seek(0.0f);
            }
            
            _isPlaying = true;
            _isPaused = false;
        }

        public void Seek(float progress) {
            _currentTime = TimeSpan.FromSeconds(TotalTime.TotalSeconds * progress);
            ReplayCommandsAtCurrentTime();
        }

        public void Pause() {
            _isPaused = true;
        }

        public void Stop() {
            Seek(1.0f);
            _isPlaying = false;
        }

        public void EraseFuture() {
            while (_futureCommands.Count > 0) {
                _futureCommands.RemoveFirst();
            }
        }

        private void RedoNextCommand() {
            ICommandSnapshot futureSnapshot = _futureCommands.First.Value.CommandSnapshot;
            futureSnapshot.Command.Run();
            
            _pastCommands.AddLast(_futureCommands.First.Value);
            _futureCommands.RemoveFirst();
        }

        private void UndoPreviousCommand() {
            ICommandSnapshot pastSnapshot = _pastCommands.Last.Value.CommandSnapshot;
            pastSnapshot.Command.Undo();

            _futureCommands.AddFirst(_pastCommands.Last.Value);
            _pastCommands.RemoveLast();
        }
    }
}
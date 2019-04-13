using System;
using System.Collections.Generic;
using CommandSystem;
using Logging;
using UnityEngine;
using Util;
using Zenject;
using ILogger = Logging.ILogger;

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

        private TimeSpan _startingTimeSpan;
        private bool _isPlaying;
        private bool _isPaused;


        public float Progress {
            get {
                if (_futureCommands.Count == 0) {
                    return 1;
                }
                
                if (_pastCommands.Count == 0) {
                    return 0;
                }

                int totalCommandCount = _pastCommands.Count + _futureCommands.Count;
                return (_pastCommands.Count / (float)totalCommandCount);
            }
        }
        
        public uint PlaybackSpeed { get; set; }

        private TimeSpan FirstTimeSpan {
            get {
                if (_pastCommands.Count > 0) {
                    return _pastCommands.First.Value.ReplayTime;
                }

                if (_futureCommands.Count > 0) {
                    return _futureCommands.First.Value.ReplayTime;
                }
                
                return TimeSpan.Zero;
            }
        }

        private TimeSpan NextCommandTime {
            get {
                if (_futureCommands.Count == 0) {
                    return TimeSpan.Zero;
                }
                
                return _futureCommands.First.Value.ReplayTime - FirstTimeSpan;
            }
        }

        private TimeSpan TimeSincePlaybackStarted {
            get {
                return _clock.Now - _startingTimeSpan;
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

            TimeSpan timeFromPreviousCommand = commandSnapshot.ExecutionTime - 
                                               _pastCommands.Last.Value.CommandSnapshot.ExecutionTime;

            if (timeFromPreviousCommand > kMaxTimeBetweenCommands) {
                return kMaxTimeBetweenCommands;
            }

            return timeFromPreviousCommand;
        }

        public void Tick() {
            if (!_isPlaying) {
                return;
            }

            if (_isPaused) {
                return;
            }
            
            if (_futureCommands.Count == 0) {
                Stop();
                return;
            }
            
            if (TimeSincePlaybackStarted < NextCommandTime) {
                return;
            }
            
            // If we are here, we need to process the next command
            RedoNextCommand();
        }

        public void Play() {
            // Rewind to the very first frame if we are at the end.
            if (!_isPlaying && (Progress.Equals(0.0f) || Progress.Equals(1.0f))) {
                _startingTimeSpan = _clock.Now;
                Seek(0.0f);
            }
            
            _isPlaying = true;
            _isPaused = false;
        }

        public void Seek(float progress) {
            int totalCommands = _futureCommands.Count + _pastCommands.Count;
            int commandToSeekTo = Mathf.RoundToInt(totalCommands * progress);
            
            while (_pastCommands.Count > 0 && commandToSeekTo < _pastCommands.Count) {
                UndoPreviousCommand();
            }

            while (_futureCommands.Count > 0 && commandToSeekTo > _pastCommands.Count) {
                RedoNextCommand();
            }
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
            futureSnapshot.Redo();
            
            _pastCommands.AddLast(_futureCommands.First.Value);
            _futureCommands.RemoveFirst();
        }

        private void UndoPreviousCommand() {
            ICommandSnapshot pastSnapshot = _pastCommands.Last.Value.CommandSnapshot;
            pastSnapshot.Undo();

            _futureCommands.AddFirst(_pastCommands.Last.Value);
            _pastCommands.RemoveLast();
        }
    }
}
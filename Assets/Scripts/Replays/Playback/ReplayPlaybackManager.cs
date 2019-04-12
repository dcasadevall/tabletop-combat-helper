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
        
        private readonly ICommandQueue _commandQueue;
        private readonly ICommandFactory _commandFactory;
        private readonly IClock _clock;
        private readonly ILogger _logger;

        private TimeSpan _clippingOffset;
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

                TimeSpan totalTimeSpan = _futureCommands.Last.Value.ExecutionTime - _pastCommands.First.Value.ExecutionTime;
                TimeSpan timeSinceStarted = _clock.Now - _startingTimeSpan;
                return (float)(timeSinceStarted.TotalSeconds / totalTimeSpan.TotalSeconds);
            }
        }
        
        public uint PlaybackSpeed { get; set; }

        private TimeSpan FirstTimeSpan {
            get {
                if (_pastCommands.Count > 0) {
                    return _pastCommands.First.Value.ExecutionTime;
                }

                if (_futureCommands.Count > 0) {
                    return _futureCommands.First.Value.ExecutionTime;
                }
                
                return TimeSpan.Zero;
            }
        }

        private TimeSpan NextCommandTime {
            get {
                if (_futureCommands.Count == 0) {
                    return TimeSpan.Zero;
                }
                
                return _futureCommands.First.Value.ExecutionTime - FirstTimeSpan + _clippingOffset;
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
        private readonly LinkedList<ICommandSnapshot> _pastCommands = new LinkedList<ICommandSnapshot>();
        
        /// <summary>
        /// Queue containing commands that have not yet been executed, but will be executed as the playback continues.
        /// </summary>
        private readonly LinkedList<ICommandSnapshot> _futureCommands = new LinkedList<ICommandSnapshot>();

        public ReplayPlaybackManager(ICommandQueue commandQueue, IClock clock, ILogger logger) {
            _commandQueue = commandQueue;
            _clock = clock;
            _logger = logger;
            
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
                ExecuteNextCommand();
            }

            // Enqueue the command snapshot.
            _pastCommands.AddLast(commandSnapshot);
        }

        public void Tick() {
            if (!_isPlaying) {
                return;
            }

            if (_isPaused) {
                return;
            }
            
            if (_futureCommands.Count == 0) {
                Pause();
                return;
            }
            
            // "Accelerate" playback by fast forwarding to a minimum span before the net command
            if (NextCommandTime - TimeSincePlaybackStarted > kMaxTimeBetweenCommands) {
                TimeSpan addedOffset = (NextCommandTime - _clock.Now) - kMaxTimeBetweenCommands;
                _clippingOffset += addedOffset;
                _logger.Log(LoggedFeature.Replays, "Adding Clipping offset: {0}", addedOffset);
            }
            
            if (TimeSincePlaybackStarted < NextCommandTime) {
                return;
            }
            
            // If we are here, we need to process the next command
            ExecuteNextCommand();
        }

        public void Play() {
            if (!_isPlaying) {
                _startingTimeSpan = _clock.Now;
                _clippingOffset = TimeSpan.Zero;

                while (_pastCommands.Count > 0) {
                    UndoPreviousCommand();
                }
            }
            
            _isPlaying = true;
            _isPaused = false;
        }

        public void Seek(float progress) {
            throw new NotImplementedException();
        }

        public void Pause() {
            _isPaused = true;
        }

        public void Stop() {
            // TODO: Use seek
            while (_futureCommands.Count > 0) {
                ExecuteNextCommand();
            }
            
            _isPlaying = false;
        }

        public void EraseFuture() {
            while (_futureCommands.Count > 0) {
                _futureCommands.RemoveFirst();
            }
        }

        private void ExecuteNextCommand() {
            ICommandSnapshot futureSnapshot = _futureCommands.First.Value;
            futureSnapshot.Redo();
            
            _pastCommands.AddLast(_futureCommands.First.Value);
            _futureCommands.RemoveFirst();
        }

        private void UndoPreviousCommand() {
            ICommandSnapshot pastSnapshot = _pastCommands.Last.Value;
            pastSnapshot.Undo();

            _futureCommands.AddFirst(_pastCommands.Last.Value);
            _pastCommands.RemoveLast();
        }
    }
}
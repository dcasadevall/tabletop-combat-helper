using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CommandSystem;
using Util;
using Zenject;

namespace Replays.Playback {
    public class ReplayPlaybackManager : ITickable, IInitializable, IReplayPlaybackManager {
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

                TimeSpan totalTimeSpan = _futureCommands.Last.Value.timeSpan - _pastCommands.First.Value.timeSpan;
                TimeSpan timeSinceStarted = _clock.Now - _startingTimeSpan;
                return (float)(timeSinceStarted.TotalSeconds / totalTimeSpan.TotalSeconds);
            }
        }
        
        public uint PlaybackSpeed { get; set; }

        private TimeSpan FirstTimeSpan {
            get {
                if (_pastCommands.Count > 0) {
                    return _pastCommands.First.Value.timeSpan;
                }

                if (_futureCommands.Count > 0) {
                    return _futureCommands.First.Value.timeSpan;
                }
                
                return TimeSpan.Zero;
            }
        }
        
        /// <summary>
        /// Queue containing commands that have been executed up to this point
        /// </summary>
        private readonly LinkedList<CommandSnapshot> _pastCommands = new LinkedList<CommandSnapshot>();
        
        /// <summary>
        /// Queue containing commands that have not yet been executed, but will be executed as the playback continues.
        /// </summary>
        private readonly LinkedList<CommandSnapshot> _futureCommands = new LinkedList<CommandSnapshot>();

        public ReplayPlaybackManager(ICommandQueue commandQueue, IClock clock) {
            _commandQueue = commandQueue;
            _clock = clock;
        }
        
        public void Initialize() {
            _commandQueue.commandQueued += HandleCommandQueued;
        }

        private void HandleCommandQueued(ICommand<ISerializable> command, ISerializable data) {
            // Make sure we do not have commands in future before we queue up past commands.
            // This is a safety net.
            // One should explicitly call Stop() or EraseFuture(), before any new command is queued.
            // to be dumped.
            // TODO: Use seek
            while (_futureCommands.Count > 0) {
                ExecuteNextCommand();
            }

            // Enqueue the command snapshot.
            CommandSnapshot commandSnapshot = new CommandSnapshot(command, data, _clock.Now);
            _pastCommands.AddLast(commandSnapshot);
        }

        public void Tick() {
            if (!_isPlaying) {
                return;
            }
            
            if (_futureCommands.Count == 0) {
                Pause();
                return;
            }
            
            TimeSpan timeSinceStarted = _clock.Now - _startingTimeSpan;
            TimeSpan nextCommandTime = _futureCommands.First.Value.timeSpan - FirstTimeSpan;
            if (timeSinceStarted < nextCommandTime) {
                return;
            }
            
            // If we are here, we need to process the next command
            ExecuteNextCommand();
        }

        public void Play() {
            if (!_isPlaying) {
                _startingTimeSpan = _clock.Now;
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
            // Execute the command directly, without going through the command queue.
            // This avoids the command queue event being triggered again
            CommandSnapshot futureSnapshot = _futureCommands.First.Value;
            futureSnapshot.command.Run(futureSnapshot.data);
            
            _pastCommands.AddLast(_futureCommands.First);
            _futureCommands.RemoveFirst();
        }
    }
}
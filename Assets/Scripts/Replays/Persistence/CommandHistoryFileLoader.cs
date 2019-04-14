using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using CommandSystem;
using Logging;
using Map.Commands;
using NSubstitute.ReturnsExtensions;
using UniRx;
using UnityEngine;
using Util;
using Zenject;
using ILogger = Logging.ILogger;

namespace Replays.Persistence {
    public class CommandHistoryFileLoader : ICommandHistoryLoader, ITickable {
        private readonly PersistenceLayerSettings _settings;
        private readonly ICommandQueue _commandQueue;
        private readonly IClock _clock;
        private readonly ILogger _logger;

        /// <summary>
        ///  GETTTTOOOOO remove
        /// </summary>
        private Queue<SerializableCommand> _pendingCommands = new Queue<SerializableCommand>();

        public IEnumerable<string> SavedCommandHistories {
            get {
                string replayDirectory = Path.Combine(Application.persistentDataPath, _settings.savePath);
                if (!Directory.Exists(replayDirectory)) {
                    return new string[0];
                }

                return Directory.EnumerateFiles(replayDirectory, "*", SearchOption.AllDirectories)
                                .Select(Path.GetFileName);
            }
        }

        public CommandHistoryFileLoader(PersistenceLayerSettings settings, ICommandQueue commandQueue, IClock clock,
                                        ILogger logger) {
            _settings = settings;
            _commandQueue = commandQueue;
            _clock = clock;
            _logger = logger;
        }

        public void LoadCommandHistory(string saveName) {
            string savePath = Path.Combine(Application.persistentDataPath,
                                           _settings.savePath,
                                           saveName);

            if (!File.Exists(savePath)) {
                _logger.LogError(LoggedFeature.Replays, "Save not found: {0}", savePath);
                return;
            }
            
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fileStream = File.Open (savePath, FileMode.Open)) {
                EnqueueCommandHistory((SerializableCommandHistory) binaryFormatter.Deserialize(fileStream));
            }
        }

        private void EnqueueCommandHistory(SerializableCommandHistory commandHistory) {
            foreach (var command in commandHistory.Commands) {
                _pendingCommands.Enqueue(command);
            }
        }

        // THIS WHOLE THING IS HACKY AND NEEDS TO DIE.
        // We need the concept of async commands
        private TimeSpan _timeToPop = TimeSpan.Zero;
        public void Tick() {
            if (_pendingCommands.Count == 0) {
                return;
            }

            if (_clock.Now < _timeToPop) {
                return;
            }
            
            SerializableCommand nextCommand = _pendingCommands.Dequeue(); 
            _commandQueue.Enqueue(nextCommand.commandType, nextCommand.dataType, nextCommand.data);

            if (nextCommand.commandType == typeof(LoadMapCommand)) {
                _timeToPop = _clock.Now + TimeSpan.FromSeconds(2);
            }
        }
    }
}
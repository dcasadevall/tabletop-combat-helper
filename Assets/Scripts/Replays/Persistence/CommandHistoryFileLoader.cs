using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using CommandSystem;
using Logging;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Replays.Persistence {
    public class CommandHistoryFileLoader : ICommandHistoryLoader {
        private readonly PersistenceLayerSettings _settings;
        private readonly ICommandQueue _commandQueue;
        private readonly ILogger _logger;

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

        public CommandHistoryFileLoader(PersistenceLayerSettings settings, ICommandQueue commandQueue,
                                        ILogger logger) {
            _settings = settings;
            _commandQueue = commandQueue;
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
                _commandQueue.Enqueue(command.commandType, command.dataType, command.data, CommandSource.SavedReplay);
            }
        }
    }
}
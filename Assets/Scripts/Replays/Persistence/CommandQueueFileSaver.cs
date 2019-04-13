using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CommandSystem;
using Logging;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Replays.Persistence {
    public class CommandHistoryFileSaver : ICommandHistorySaver, ICommandQueueListener, IInitializable {
        private readonly ICommandQueue _commandQueue;
        private readonly ILogger _logger;
        private readonly PersistenceLayerSettings _settings;
        private readonly SerializableCommandHistory _serializableCommandHistory = new SerializableCommandHistory();

        public CommandHistoryFileSaver(ICommandQueue commandQueue, ILogger logger, PersistenceLayerSettings settings) {
            _commandQueue = commandQueue;
            _logger = logger;
            _settings = settings;
        }

        public void Initialize() {
            _commandQueue.AddListener(this);
        }
        
        public CommandHistorySaveInfo SaveCommandHistory(string name) {
            CreateReplaysDirectoryIfNecessary();
            string savePath = Path.Combine(Application.persistentDataPath, _settings.savePath, name);
            int duplicateIndex = 1;
            while (File.Exists(savePath)) {
                savePath = Path.Combine(Application.persistentDataPath,
                                        _settings.savePath,
                                        string.Format(_settings.duplicateFormat, name, duplicateIndex));
                duplicateIndex++;
            }
            
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fileStream = File.Open (savePath, FileMode.OpenOrCreate)) {
                binaryFormatter.Serialize(fileStream, _serializableCommandHistory);
            } 
            
            _logger.Log(LoggedFeature.Replays, "Successfully saved replay to: {0}", savePath);
            return new CommandHistorySaveInfo(Path.GetFileName(savePath));
        }

        private void CreateReplaysDirectoryIfNecessary() {
            string replayDirectory = Path.Combine(Application.persistentDataPath, _settings.savePath);
            if (File.Exists(replayDirectory)) {
                return;
            }

            Directory.CreateDirectory(replayDirectory);
        }

        public void HandleCommandQueued(ICommandSnapshot commandSnapshot) {
            _serializableCommandHistory.AddCommandSnapshot(commandSnapshot);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logging;
using Zenject;
using ILogger = Logging.ILogger;

namespace CommandSystem {
    public class CommandFactory : ICommandFactory {
        private readonly ILogger _logger;
        private readonly HashSet<DiContainer> _containers = new HashSet<DiContainer>();

        public CommandFactory(DiContainer container, ILogger logger) {
            _logger = logger;
            _containers.Add(container);
        }

        public void RegisterSceneContainer(DiContainer container) {
            _containers.Add(container);
        }

        public ICommand Create(Type commandType, Type dataType, ISerializable data) {
            foreach (var container in _containers) {
                if (!container.HasBinding(commandType)) {
                    continue;
                }
                
                // TODO: Avoid using instantiate explicit here
                ICommand command = (ICommand)container.InstantiateExplicit(commandType, new List<TypeValuePair> {
                    new TypeValuePair(dataType, data)
                });
                
                if (command != null) {
                    return command;
                }
            }

            _logger.LogError(LoggedFeature.CommandSystem,
                             "Command not found in registered contexts: {0}",
                             typeof(ICommand));
            return null;
        }
    }
}
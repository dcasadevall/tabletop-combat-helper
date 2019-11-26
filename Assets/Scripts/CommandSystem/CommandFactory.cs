using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logging;
using Zenject;
using ILogger = Logging.ILogger;

namespace CommandSystem {
    public class CommandFactory : ICommandFactory {
        private static readonly HashSet<DiContainer> _containers = new HashSet<DiContainer>();
        
        private readonly DiContainer _container;
        private readonly ILogger _logger;

        public CommandFactory(DiContainer container, ILogger logger) {
            _container = container;
            _logger = logger;
        }

        public static void RegisterSceneContainer(DiContainer container) {
            _containers.Add(container);
        }

        public ICommand Create(Type commandType, Type dataType, ISerializable data) {
            // If our container has the binding in context, we prioritize that one
            if (_container.HasBinding(commandType)) {
                return (ICommand)_container.InstantiateExplicit(commandType, new List<TypeValuePair> {
                    new TypeValuePair(dataType, data)
                });   
            }
            
            // Otherwise, we iterate through the containers statically injected, which may not be in context.
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
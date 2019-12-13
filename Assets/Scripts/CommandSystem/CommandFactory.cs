using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logging;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace CommandSystem {
    public class CommandFactory : ICommandFactory {
        private static readonly HashSet<DiContainer> _containers = new HashSet<DiContainer>();
        
        private readonly ILogger _logger;

        public CommandFactory(DiContainer container, ILogger logger) {
            _logger = logger;
            RegisterSceneContainer(container);
        }

        public static void RegisterSceneContainer(DiContainer container) {
            _containers.Add(container);
        }

        public static void UnregisterSceneContainer(DiContainer container) {
            _containers.Remove(container);
        }

        public ICommand Create(Type commandType, Type dataType, ISerializable data) {
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
                             commandType);
            return null;
        }
    }
}
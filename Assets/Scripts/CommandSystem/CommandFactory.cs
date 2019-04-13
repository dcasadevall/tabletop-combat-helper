using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logging;
using UnityEngine;
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

        public ICommand<TData> Create<TData>() where TData : ISerializable {
            foreach (var diContainer in _containers) {
                ICommand<TData> command = diContainer.TryResolve<ICommand<TData>>();
                if (command != null) {
                    return command;
                }
            }

            _logger.LogError(LoggedFeature.CommandSystem,
                             "Command not found in registered contexts: {0}",
                             typeof(ICommand<TData>));
            return null;
        }

        public ICommand Create(Type commandType) {
            foreach (var diContainer in _containers) {
                try {
                    Type resolvedType = diContainer.ResolveType(commandType);
                    if (resolvedType != null) {
                        // return new GenericCommandAdapter((ICommand)diContainer.Instantiate(resolvedType));
                        return (ICommand) diContainer.Instantiate(resolvedType);
                    }
                } catch (Exception e) { }
            }

            _logger.LogError(LoggedFeature.CommandSystem, "Command not found in registered contexts: {0}", commandType);
            return null;
        }
    }
}
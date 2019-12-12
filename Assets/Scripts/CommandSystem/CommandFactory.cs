using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logging;
using Zenject;
using ILogger = Logging.ILogger;

namespace CommandSystem {
    public class CommandFactory : ICommandFactory {
//        private static readonly HashSet<DiContainer> _containers = new HashSet<DiContainer>();
//        
        private readonly DiContainer _container;
        private readonly ILogger _logger;

        public CommandFactory(DiContainer container, ILogger logger) {
            _container = container;
            _logger = logger;
        }

//        internal ConcreteIdBinderGeneric<TCommand> Bind<TCommand>() where TCommand : ICommand {
//            return _container.Bind<TCommand>();
//        }
//        
//        internal bool Unbind(Type commandType) {
//            if (!typeof(ICommand).IsAssignableFrom(commandType)) {
//                return false;
//            }
//            
//            return _container.Unbind(commandType);
//        }
        
        public ICommand Create(Type commandType, Type dataType, ISerializable data) {
            ICommand command = (ICommand)_container.InstantiateExplicit(commandType, new List<TypeValuePair> {
                new TypeValuePair(dataType, data)
            });
                
            if (command == null) {
                _logger.LogError(LoggedFeature.CommandSystem,
                                 "Command not found in registered contexts: {0}",
                                 typeof(ICommand));
            }

            return command;
        }
    }
}
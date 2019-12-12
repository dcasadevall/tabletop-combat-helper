using System;
using System.Collections.Generic;
using Logging;
using Zenject;

namespace CommandSystem.Installers {
    public sealed class CommandBinder : ITickable, IDisposable {
        private readonly ILogger _logger;
        private readonly HashSet<CommandBinding> _activeBindings = new HashSet<CommandBinding>();
        private readonly HashSet<CommandBinding> _inactiveBindings = new HashSet<CommandBinding>();

        internal CommandBinder(ILogger logger) {
            _logger = logger;
        }

        /// <summary>
        /// Installs the CommandsInstaller of type <see cref="TInstaller"/>, with the given container.
        /// The given <see cref="isActiveFunc"/> will be polled so we can <see cref="UninstallBindings"/> when
        /// this installer is no longer active.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="afterBind"></param>
        /// <param name="isActiveFunc"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        public void BindCommand<TCommand>(DiContainer container,
                                          Action<ConcreteIdBinderGeneric<ICommand>> afterBind,
                                          Func<bool> isActiveFunc)
            where TCommand : ICommand {
            _activeBindings.Add(new CommandBinding(container, typeof(TCommand), afterBind, isActiveFunc, _logger));
        }

        public void Tick() {
            foreach (var binding in _activeBindings) {
                if (!binding.isActiveFunc()) {
                    binding.Unbind();
                    _inactiveBindings.Add(binding);
                } else {
                    binding.Bind();
                    _activeBindings.Add(binding);
                }
            }

            _activeBindings.RemoveWhere(binding => !binding.isActiveFunc());
            _inactiveBindings.RemoveWhere(binding => binding.isActiveFunc());
        }

        public void Dispose() {
            foreach (var commandBinding in _activeBindings) {
                commandBinding.Unbind();
            }

            _activeBindings.Clear();
            _inactiveBindings.Clear();
        }

        private class CommandBinding {
            private readonly CommandsInstaller _commandsInstaller;
            private readonly DiContainer _container;
            private readonly Type _bindingType;
            private readonly Action<ConcreteIdBinderGeneric<ICommand>> _afterBindCallback;
            public readonly Func<bool> isActiveFunc;
            private readonly ILogger _logger;

            public CommandBinding(DiContainer container,
                                  Type bindingType,
                                  Action<ConcreteIdBinderGeneric<ICommand>> afterBindCallback,
                                  Func<bool> isActiveFunc,
                                  ILogger logger) {
                _container = container;
                _bindingType = bindingType;
                _afterBindCallback = afterBindCallback;
                this.isActiveFunc = isActiveFunc;
                _logger = logger;
            }

            public void Bind() {
                _afterBindCallback.Invoke(_container.Bind<ICommand>());
            }

            public void Unbind() {
                if (!_container.Unbind(_bindingType)) {
                    _logger.LogError(LoggedFeature.CommandSystem, "Unbinding already unbound command: {0}", _bindingType);
                }
            }
        }
    }
}
using System;
using Logging;

namespace InputSystem {
    public class InputLock : IInputLock {
        public event Action InputLockAcquired;
        public event Action InputLockReleased;

        public bool IsLocked {
            get {
                lock (this) {
                    return _owner != null;
                }
            }
        }

        private readonly ILogger _logger;
        private Guid? _owner;

        public InputLock(ILogger logger) {
            _logger = logger;
        }

        public IDisposable Lock() {
            lock (this) {
                if (_owner != null) {
                    var msg = $"Failed to acquire input lock. Already locked by: {_owner}";
                    _logger.LogError(LoggedFeature.Input, msg);
                    throw new Exception(msg);
                }

                var lockToken = new LockToken(this);
                _owner = lockToken.guid;
                _logger.Log(LoggedFeature.Input, "Lock acquired: {0}", _owner);
                InputLockAcquired?.Invoke();

                return lockToken;
            }
        }

        private void Unlock(Guid requestor) {
            lock (this) {
                if (!_owner.Equals(requestor)) {
                    var msg = $"Failed to release input lock. Lock owner: {_owner}. Requestor: {requestor}";
                    _logger.LogError(LoggedFeature.Input, msg);
                    throw new Exception(msg);
                }

                _logger.Log(LoggedFeature.UI, "Lock released: {0}", _owner);
                _owner = null;
                InputLockReleased?.Invoke();
            }
        }

        private class LockToken : IDisposable {
            public readonly Guid guid;
            private readonly InputLock _inputLock;

            public LockToken(InputLock inputLock) {
                _inputLock = inputLock;
                guid = Guid.NewGuid();
            }

            public void Dispose() {
                _inputLock.Unlock(guid);
            }
        }
    }
}
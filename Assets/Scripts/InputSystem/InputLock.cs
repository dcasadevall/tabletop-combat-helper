using System;

namespace InputSystem {
    public class InputLock : IInputLock {
        public event Action InputLockAcquired;
        public event Action InputLockReleased;

        public bool IsLocked {
            get {
                return _owner != null;
            }
        }
        
        private Guid? _owner;
        
        public Guid? Lock() {
            if (IsLocked) {
                return null;
            }
            
            _owner = Guid.NewGuid();
            InputLockAcquired?.Invoke();
            
            return _owner;
        }

        public bool Unlock(Guid owner) {
            if (_owner == null) {
                return false;
            }
            
            if (!_owner.Equals(owner)) {
                return false;
            }

            _owner = null;
            InputLockReleased?.Invoke();
            
            return true;
        }
    }
}
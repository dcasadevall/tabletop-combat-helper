using System;

namespace InputSystem {
    /// <summary>
    /// A globally accessible input lock that can be used to prevent other actors responding to input to
    /// do so when anyone has claimed this lock.
    ///
    /// This allows us to avoid conflict between multiple actors that respond to the same input.
    /// </summary>
    public interface IInputLock {
        /// <summary>
        /// Triggered when any actor acquires the input lock.
        /// </summary>
        event Action InputLockAcquired;
        /// <summary>
        /// Triggered when the actor currently holding the input lock releases it.
        /// </summary>
        event Action InputLockReleased;
        
        /// <summary>
        /// True if the lock has been acquired by any actor.
        /// </summary>
        bool IsLocked { get; }
        /// <summary>
        /// Attempts to acquire the lock, returning a <see cref="IDisposable"/> object that will release the lock
        /// when disposed of.
        ///
        /// This will throw an exception if the lock is already held by another actor. So be sure to check for
        /// <see cref="IsLocked"/> if you want to fail gracefully.
        /// </summary>
        /// <returns></returns>
        IDisposable Lock();
    }
}
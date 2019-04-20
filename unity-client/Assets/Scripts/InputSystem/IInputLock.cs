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
        /// True if the lock has been acquired by any actor.
        /// </summary>
        bool IsLocked { get; }
        /// <summary>
        /// Attempts to acquire the lock, returning a new Guid if successful, or null otherwise.
        /// </summary>
        /// <returns></returns>
        Guid? Lock();
        /// <summary>
        /// Unlocks the current lock if the given owner matches the owner that originally locked it.
        /// Returns true if successful, and false otherwise.
        /// </summary>
        /// <param name="owner"></param>
        bool Unlock(Guid owner);
    }
}
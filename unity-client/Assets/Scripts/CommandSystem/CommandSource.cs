
namespace CommandSystem {
    /// <summary>
    /// A class defining the source for a command being queued with <see cref="ICommandQueue"/>.
    /// One can extend this class to specify other game sources. For now, we put them all here.
    ///
    /// This is useful so we can avoid broadcasting / requeuing certain commands in response to other commands
    /// received.
    /// </summary>
    public struct CommandSource {
        #region Values
        public static CommandSource Game => new CommandSource("Game");
        public static CommandSource SavedReplay => new CommandSource("SavedReplay");
        public static CommandSource Network => new CommandSource("Network");
        #endregion
        
        public readonly string name;
        public CommandSource(string name) {
            this.name = name;
        }
        
        #region Equality Methods
        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            CommandSource other = (CommandSource)obj;
            return other.name.Equals(name);
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }
        #endregion

        #region string methods
        public override string ToString() {
            return name;
        }
        #endregion

        #region Operators
        // This allows casting to and from string
        public static implicit operator string(CommandSource commandSource) {
            return commandSource.name;
        }

        public static implicit operator CommandSource(string name) {
            return new CommandSource(name);
        }

        public static bool operator ==(CommandSource a, CommandSource b) {
            return a.name.Equals(b.name);
        }

        public static bool operator !=(CommandSource a, CommandSource b) {
            return !a.name.Equals(b.name);
        }
        #endregion
    }

}
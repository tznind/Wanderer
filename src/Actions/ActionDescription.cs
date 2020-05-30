using System;

namespace Wanderer.Actions
{
    /// <summary>
    /// User understandable representation of an <see cref="IAction"/>
    /// </summary>
    public class ActionDescription
    {
        /// <summary>
        /// Single key to press to trigger this action (if supported by <see cref="IUserinterface"/>)
        /// </summary>
        public char HotKey { get; set; }

        /// <summary>
        /// User understandable name for the underlying action
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Equality based on <see cref="Name"/> and <see cref="HotKey"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool Equals(ActionDescription other)
        {
            return HotKey == other.HotKey && Name == other.Name;
        }
        /// <summary>
        /// Equality based on <see cref="Name"/> and <see cref="HotKey"/>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ActionDescription) obj);
        }
        /// <summary>
        /// Equality based on <see cref="Name"/> and <see cref="HotKey"/>
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return (HotKey.GetHashCode() * 397) ^ (Name != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0);
            }
        }

        /// <summary>
        /// True if the description matches the provided <paramref name="action"/>
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool Matches(IAction action)
        {
            return
                string.Equals(action.Name, Name, StringComparison.CurrentCultureIgnoreCase)
                &&
                action.HotKey == HotKey;
        }
    }
}
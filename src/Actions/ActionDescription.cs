using System;

namespace Wanderer.Actions
{
    public class ActionDescription
    {
        public char HotKey { get; set; }
        public string Name { get; set; }

        protected bool Equals(ActionDescription other)
        {
            return HotKey == other.HotKey && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ActionDescription) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HotKey, Name);
        }

        public bool Matches(IAction action)
        {
            return
                string.Equals(action.Name, Name, StringComparison.CurrentCultureIgnoreCase)
                &&
                action.HotKey == HotKey;
        }
    }
}
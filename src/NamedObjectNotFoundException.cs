using System;

namespace Wanderer
{
    public class NamedObjectNotFoundException : Exception
    {
        public string Name { get; }
        public NamedObjectNotFoundException(string message, string name): this(message,null,name)
        {
        }
        public NamedObjectNotFoundException(string message,Exception ex, string name): base(message,ex)
        {
            Name = name;
        }
    }
}
using System;

namespace Wanderer
{
    /// <summary>
    /// Thrown when there is a request to reference something by Guid but no matching object is found
    /// </summary>
    public class GuidNotFoundException : Exception
    {
        public Guid Guid { get; }

        public GuidNotFoundException(string message,Guid guid) : this(message,null,guid)
        {
        }
        public GuidNotFoundException(string message,Exception innerException,Guid guid) : base(message,innerException)
        {
            Guid = guid;
        }
    }
}
using System;

namespace Wanderer
{
    /// <summary>
    /// Thrown when there is a request to reference something by Guid but no matching object is found
    /// </summary>
    public class GuidNotFoundException : Exception
    {
        /// <summary>
        /// The identifier that was being sought
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Creates a new instance of the Exception documenting that the codebase was unable to find the referenced <paramref name="guid"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="guid"></param>
        public GuidNotFoundException(string message,Guid guid) : this(message,null,guid)
        {
        }

        /// <summary>
        /// Creates a new instance of the Exception documenting that the codebase was unable to find the referenced <paramref name="guid"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="guid"></param>
        public GuidNotFoundException(string message,Exception innerException,Guid guid) : base(message,innerException)
        {
            Guid = guid;
        }
    }
}
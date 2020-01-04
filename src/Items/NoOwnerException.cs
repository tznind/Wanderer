using System;

namespace StarshipWanderer.Items
{
    public class NoOwnerException : Exception
    {
        public NoOwnerException(string message) :base(message)
        {
        }

        public NoOwnerException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
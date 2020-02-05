using System;

namespace StarshipWanderer.Compilation
{
    public class PropertyChainException : Exception
    {
        public PropertyChainException(string message):base(message)
        {
        }
    }
}
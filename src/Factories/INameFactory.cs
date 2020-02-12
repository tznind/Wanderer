using System;

namespace Wanderer.Factories
{
    public interface INameFactory
    {
        string GenerateName(Random r);
    }
}
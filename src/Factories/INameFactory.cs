using System;

namespace StarshipWanderer.Factories
{
    public interface INameFactory
    {
        string GenerateName(Random r);
    }
}
using System;

namespace StarshipWanderer.Actors
{
    public interface INameFactory
    {
        string GenerateName(Random r);
    }
}
using System.Collections.Generic;
using System.Linq;

namespace StarshipWanderer.Actors
{
    public class NameFactory : INameFactory
    {
        public string[] Forenames { get; set; }
        public string[] Surnames { get; set; }

        public NameFactory(IEnumerable<string> forenames,IEnumerable<string> surnames)
        {
            Forenames = forenames.ToArray();
            Surnames = surnames.ToArray();
        }

        public string GenerateName(IActor suitableFor)
        {
            return Forenames[suitableFor.CurrentLocation.World.R.Next(Forenames.Length)] + " " +
                   Surnames[suitableFor.CurrentLocation.World.R.Next(Surnames.Length)];
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarshipWanderer.Factories
{
    public class NameFactory : INameFactory
    {
        public string[] Forenames { get; set; }
        public string[] Surnames { get; set; }

        public NameFactory(IEnumerable<string> forenames,IEnumerable<string> surnames)
        {
            Forenames = forenames?.ToArray()?? new string[0];
            Surnames = surnames?.ToArray() ?? new string[0];
        }

        public string GenerateName(Random r)
        {
            return 
                (
                    (Forenames.Any() ? Forenames[r.Next(Forenames.Length)] : "") 
                    + " " +
                    (Surnames.Any()? Surnames[r.Next(Surnames.Length)]: "")
                )
                .Trim();
        }
    }
}
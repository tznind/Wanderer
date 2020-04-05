using System;
using System.Collections.Generic;

namespace Wanderer.Systems
{
    public abstract class System : ISystem
    {
        /// <summary>
        /// Human readable name for the system
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier for this system so that it can be referenced from scripts
        /// etc.  This should be a constant (Don't use NewGuid!).  When sub-classing it is
        /// permissible to use the parents guid if you are semantically the same (e.g. subclass
        /// methods are alternate ways to load the system)
        /// </summary>
        public Guid Identifier { get; set; }

        public abstract void Apply(SystemArgs args);

        public void ApplyToAll(IEnumerable<IHasStats> recipients, SystemArgs args)
        {
            foreach (IHasStats r in recipients)
            {
                args.Recipient = r;
                Apply(args);
            }
        }


    }
}
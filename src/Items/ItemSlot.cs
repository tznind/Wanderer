using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Adjectives.ActorOnly;

namespace Wanderer.Items
{
    public class ItemSlot : IItemSlot
    {
        public string Name { get; set; }
        
        public int NumberRequired { get; set; }
        public InjuryRegion[] SensitiveTo { get; set; }

        public ItemSlot()
        {

        }

        public ItemSlot(string name,int numberRequired, params InjuryRegion[] sensitiveTo)
        {
            NumberRequired = numberRequired;
            Name = name;
            SensitiveTo = sensitiveTo;
        }
    }
}
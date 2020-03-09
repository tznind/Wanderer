using System;
using System.Collections.Generic;

namespace Wanderer.Systems
{
    public class Resistances
    {
        public List<Type> Vulnerable {get; set; } = new List<Type>();

        public List<Type> Resist {get; set; } = new List<Type>();
        
        public List<Type> Immune {get; set; } = new List<Type>();
        
    }
}
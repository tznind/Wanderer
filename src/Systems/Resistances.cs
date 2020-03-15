using System;
using System.Collections.Generic;

namespace Wanderer.Systems
{
    public class Resistances
    {
        public List<string> Vulnerable {get; set; } = new List<string>();

        public List<string> Resist {get; set; } = new List<string>();
        
        public List<string> Immune {get; set; } = new List<string>();
        
    }
}
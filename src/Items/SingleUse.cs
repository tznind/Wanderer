using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Wanderer.Adjectives;

namespace Wanderer.Items
{
    public class SingleUse : Adjective
    {
        [JsonConstructor]
        protected SingleUse()
        {
            
        }

        public SingleUse(IItem owner) : base(owner)
        {
            BaseBehaviours.Add(new SingleUseBehaviour(this));
            IsPrefix = false;
        }
    }
}
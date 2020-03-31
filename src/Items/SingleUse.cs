using System;
using System.Collections.Generic;
using Wanderer.Adjectives;

namespace Wanderer.Items
{
    public class SingleUse : Adjective
    {
        public IItem OwnerItem { get; set; }

        public SingleUse(IItem owner) : base(owner)
        {
            OwnerItem = owner;
            BaseBehaviours.Add(new SingleUseBehaviour(this));
            IsPrefix = false;
        }
    }
}
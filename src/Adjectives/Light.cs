using System.Collections.Generic;
using Wanderer.Items;

namespace Wanderer.Adjectives
{
    public class Light : Adjective
    {

        public Light(IHasStats owner) : base(owner)
        {
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Cancels Dark";
        }
    }
}
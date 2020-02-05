using System.Collections.Generic;
using StarshipWanderer.Items;

namespace StarshipWanderer.Adjectives
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
using System.Collections.Generic;
using Wanderer.Places;

namespace Wanderer.Adjectives.RoomOnly
{
    public class Stale : Adjective
    {
        public Stale(IPlace owner) : base(owner)
        {

        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Wounds become infected faster";
        }
    }
}
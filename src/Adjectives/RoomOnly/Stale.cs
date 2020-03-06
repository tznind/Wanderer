using System.Collections.Generic;
using Wanderer.Places;

namespace Wanderer.Adjectives.RoomOnly
{
    public class Stale : Adjective
    {
        public Stale(IRoom owner) : base(owner)
        {

        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Wounds become infected faster";
        }
    }
}
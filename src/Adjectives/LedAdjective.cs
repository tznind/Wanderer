using System.Collections.Generic;
using Wanderer.Actions;

namespace Wanderer.Adjectives
{
    public class LedAdjective : Adjective
    {
        public LeadershipFrame Led { get; set; }
        public LedAdjective(LeadershipFrame led):base(led.TargetIfAny)
        {
            Led = led;
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Changes plan priorities";
        }
    }
}
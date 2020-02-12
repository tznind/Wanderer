using System.Collections.Generic;
using Wanderer.Adjectives;
using Wanderer.Stats;

namespace Wanderer.Actions.Coercion
{
    public class Coerced : Adjective
    {
        public CoerceFrame CoercedFrame { get; }

        public Coerced(CoerceFrame frame) : base(frame.TargetIfAny)
        {
            CoercedFrame = frame;
            //don't show this Adjective in it's name
            IsPrefix = false;
            BaseStats[Stat.Initiative] = 10000;
            BaseBehaviours.Add(new CoercedBehaviour(this));
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Forced to perform a specific action";
        }
    }
}
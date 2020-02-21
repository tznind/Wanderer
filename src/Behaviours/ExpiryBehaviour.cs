using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Wanderer.Adjectives;
using Wanderer.Behaviours;

namespace Wanderer.Behaviours
{
    public class ExpiryBehaviour : Behaviour
    {
        public IAdjective Adjective { get; set; }
        public int RoundsBeforeRemoval { get; set; }
        public HashSet<Guid> RoundsSeen { get; set; } = new HashSet<Guid>();

        [JsonConstructor]
        protected ExpiryBehaviour():base(null)
        {

        }

        /// <summary>
        /// Add to <see cref="IHasStats.BaseBehaviours"/> in order to expire the given <paramref name="adjective"/>
        /// after <paramref name="roundsBeforeRemoval"/>
        /// </summary>
        /// <param name="adjective"></param>
        /// <param name="roundsBeforeRemoval">1 for current round only, 2 for this round and next round, etc</param>
        public ExpiryBehaviour(IAdjective adjective, int roundsBeforeRemoval) : base(adjective.Owner)
        {
            Adjective = adjective;
            RoundsBeforeRemoval = roundsBeforeRemoval;
        }

        public override void OnRoundEnding(IWorld world,IUserinterface ui, Guid round)
        {
            RoundsSeen.Add(round);

            if (RoundsSeen.Count >= RoundsBeforeRemoval)
            {
                Adjective.Owner.Adjectives.Remove(Adjective);
                Adjective.BaseBehaviours.Remove(this);
            } 
        }

        public override bool AreIdentical(IBehaviour other)
        {
            if (other is ExpiryBehaviour o)
            {
                return
                    //timer left 
                    o.RoundsBeforeRemoval - o.RoundsSeen.Count

                    //timer left
                    == RoundsBeforeRemoval - RoundsSeen.Count;
            }

            return false;
        }
    }
}
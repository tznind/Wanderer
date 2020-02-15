using System;
using System.Collections.Generic;
using Wanderer.Adjectives;
using Wanderer.Behaviours;

namespace Wanderer.Behaviours
{
    public class ExpiryBehaviour : Behaviour
    {
        private readonly IAdjective _adjective;
        private readonly int _roundsBeforeRemoval;
        readonly HashSet<Guid> _roundsSeen = new HashSet<Guid>();

        /// <summary>
        /// Add to <see cref="IHasStats.BaseBehaviours"/> in order to expire the given <paramref name="adjective"/>
        /// after <paramref name="roundsBeforeRemoval"/>
        /// </summary>
        /// <param name="adjective"></param>
        /// <param name="roundsBeforeRemoval">1 for current round only, 2 for this round and next round, etc</param>
        public ExpiryBehaviour(IAdjective adjective, int roundsBeforeRemoval) : base(adjective.Owner)
        {
            _adjective = adjective;
            _roundsBeforeRemoval = roundsBeforeRemoval;
        }

        public override void OnRoundEnding(IWorld world,IUserinterface ui, Guid round)
        {
            _roundsSeen.Add(round);

            if (_roundsSeen.Count >= _roundsBeforeRemoval)
            {
                _adjective.Owner.Adjectives.Remove(_adjective);
                _adjective.BaseBehaviours.Remove(this);
            } 
        }

        public override bool AreIdentical(IBehaviour other)
        {
            if (other is ExpiryBehaviour o)
            {
                return
                    //timer left 
                    o._roundsBeforeRemoval - o._roundsSeen.Count

                    //timer left
                    == _roundsBeforeRemoval - _roundsSeen.Count;
            }

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer.Behaviours
{
    public class ExpiryBehaviour : Behaviour
    {
        private readonly IAdjective _adjective;
        private readonly int _roundsBeforeRemoval;
        HashSet<Guid> _roundsSeen = new HashSet<Guid>();

        /// <summary>
        /// Add to <see cref="IHasStats.BaseBehaviours"/> in order to expire the given <paramref name="adjective"/> after <paramref name="roundsBeforeRemoval"/>
        /// </summary>
        /// <param name="adjective"></param>
        /// <param name="roundsBeforeRemoval"></param>
        public ExpiryBehaviour(IAdjective adjective, int roundsBeforeRemoval) : base(adjective.Owner)
        {
            _adjective = adjective;
            _roundsBeforeRemoval = roundsBeforeRemoval;
        }

        public override void OnPush(IUserinterface ui, ActionStack stack, Frame frame)
        {
            _roundsSeen.Add(stack.Round);

            if (_roundsSeen.Count > _roundsBeforeRemoval)
            {
                _adjective.Owner.Adjectives.Remove(_adjective);
                _adjective.BaseBehaviours.Remove(this);
            }
                
        }
    }
}
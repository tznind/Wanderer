using System.Linq;
using Wanderer.Adjectives.ActorOnly;
using Wanderer.Systems;

namespace Wanderer.Behaviours
{
    public class GetsHungryBehaviour : Behaviour
    {
        private int _roundsSeen;

        public IInjurySystem HungerSystem { get; set;}
        
        public GetsHungryBehaviour(IHasStats owner,IInjurySystem hungerSystem) : base(owner)
        {
            HungerSystem = hungerSystem;
        }


        public override void OnRoundEnding(IWorld world,IUserinterface ui, System.Guid round)
        {
            //They are already hungry
            if(Owner.Adjectives.OfType<IInjured>().Any(i=>i.InjurySystem == HungerSystem))
            {
                _roundsSeen = 0;
                return;
            }

            _roundsSeen++;

            if(_roundsSeen > 5)
                HungerSystem.Apply(new SystemArgs(world,ui,1,null,Owner,round));
        }
    }
}
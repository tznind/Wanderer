using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Wanderer.Behaviours;

namespace Wanderer.Adjectives
{
    public class InjuredBehaviour : Behaviour
    {
        /// <summary>
        /// Tracks how many rounds have gone by
        /// </summary>
        readonly HashSet<Guid> _roundsSeen = new HashSet<Guid>();

        [JsonConstructor]
        protected InjuredBehaviour()
        {
            
        }

        public InjuredBehaviour(IHasStats owner) : base(owner)
        {
        }
        public override void OnPush(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            _roundsSeen.Add(stack.Round);

            var inj = (IInjured)Owner;

            //light wounds
            if (inj.InjurySystem.ShouldNaturallyHeal(inj, _roundsSeen.Count))
                inj.Heal(ui, stack.Round);
            else
                //heavy wounds
            if (inj.InjurySystem.ShouldWorsen(inj, _roundsSeen.Count))
            {
                inj.Worsen(ui, stack.Round); //make injury worse
                _roundsSeen.Clear(); //and start counting again from 0
            }
        }
        
        public override void OnRoundEnding(IWorld world,IUserinterface ui, Guid round)
        {
            var inj = (IInjured)Owner;

            if (inj.InjurySystem.HasFatalInjuries(inj, out string reason))
                inj.InjurySystem.Kill(inj, ui,round, reason);
        }

    }
}
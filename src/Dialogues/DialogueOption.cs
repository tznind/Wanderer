using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Compilation;
using Wanderer.Systems;

namespace Wanderer.Dialogues
{
    public class DialogueOption
    {
        public Guid? Destination { get; set; }

        public int? Attitude { get; set; }

        public string Text { get; set; }

        public List<IEffect> Effect = new List<IEffect>();

        public List<ICondition<SystemArgs>> Condition { get; set; } = new List<ICondition<SystemArgs>>();

        /// <summary>
        /// Set to true to allow user to select this option only once
        /// </summary>
        public bool SingleUse { get; set; } = false;

        internal bool AllConditionsMet(IWorld world, SystemArgs args)
        {
            if(Transition.HasValue)
            {
                var match  = args.Room.Get(Transition.Value).FirstOrDefault();
    
                //Cannot transition to an abscent npc or one who has nothing to say
                if(match?.Dialogue?.Next == null)
                    return false;
            }

            return Condition.All(c=>c.IsMet(args.World,args));
        }

        /// <summary>
        /// Set to true to indicate that the option shouldn't be offered again
        /// (e.g. for <see cref="SingleUse"/> options)
        /// </summary>
        public bool Exhausted { get; set; } = false;
        

        /// <summary>
        /// Makes dialogue option transition from talking to one room / actor to 
        /// another (who must be present).  This combines a condition of the guid being
        /// present and the state update if the option is picked
        /// </summary>
        public Guid? Transition { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
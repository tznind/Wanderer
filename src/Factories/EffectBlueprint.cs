using System;
using System.Collections.Generic;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    public class EffectBlueprint
    {
        public string Lua {get; set;}

        /// <summary>
        /// Kill the primary actor triggering the effect (text indicates cause of death e.g. "poison").  In dialogue it is the speaker (e.g. player), for action it is the action performer
        /// </summary> 
        public string Kill {get;set;}
        
        /// <summary>
        /// Apply the effect (Kill, Set etc) to the given object (default is Aggressor - the acting thing).  Options include Room (where room the event is taking place), Recipient (who you are talking to) etc
        /// </summary>
        public SystemArgsTarget Target { get; set; }

        /// <summary>
        /// Sets a stat or variable to a given value e.g. "MyCounter += 5".  If a Stat exists in <see cref="IWorld.AllStats"/> then Set will apply to that stat otherwise a variable will be assigned
        /// </summary>
        public string Set { get; set; }
        
        public IEnumerable<IEffect> Create()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                yield return new EffectCode(Lua);

            if(!string.IsNullOrWhiteSpace(Kill))
                yield return new KillEffect(Kill,Target);

            if (!string.IsNullOrWhiteSpace(Set))
                yield return new SetEffect(Set,Target);
        }
    }
}
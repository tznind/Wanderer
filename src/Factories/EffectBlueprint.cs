using System;
using System.Collections.Generic;
using Wanderer.Compilation;

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
        /// Kill the secondary (targetted) actor of the effect (text indicates cause of death e.g. "poison").  In dialogue it is the person being spoken to, for action it is the targetted individual.  For events involving only one subject acts the same as <see cref="Kill"/>
        /// </summary> 
        public string KillRecipient {get;set;}

        /// <summary>
        /// Sets a stat or variable to a given value e.g. "MyCounter += 5".  If a Stat exists in <see cref="IWorld.AllStats"/> then Set will apply to that stat otherwise a variable will be assigned
        /// </summary>
        public string Set { get; set; }

        /// <summary>
        /// Sets a stat or variable on the secondary (targeted object) to a given value e.g. "MyCounter += 5".  If a Stat exists in <see cref="IWorld.AllStats"/> then Set will apply to that stat otherwise a variable will be assigned
        /// </summary>
        public string SetRecipient { get; set; }



        public IEnumerable<IEffect> Create()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                yield return new EffectCode(Lua);

            if(!string.IsNullOrWhiteSpace(Kill))
                yield return new KillEffect(Kill);

            if(!string.IsNullOrWhiteSpace(KillRecipient))
                yield return new KillEffect(KillRecipient){RecipientOnly = true};

            if (!string.IsNullOrWhiteSpace(Set))
                yield return new SetEffect(Set);

            if (!string.IsNullOrWhiteSpace(SetRecipient))
                yield return new SetEffect(SetRecipient){RecipientOnly = true};

            
        }
    }
}
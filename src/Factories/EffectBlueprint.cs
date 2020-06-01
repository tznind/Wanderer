using System;
using System.Collections.Generic;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    /// <inheritdoc cref="IEffect"/>
    public class EffectBlueprint
    {
        /// <summary>
        /// Lua code to execute, can do anything.  Global variables include 'World', 'Recipient' etc (see github Cookbook for more info: https://github.com/tznind/Wanderer/blob/master/Cookbook.md)
        /// </summary>
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

        /// <summary>
        /// Marks the given point on the map as visible.  Combine with Room FixedLocation to help the player find interesting rooms
        /// </summary>
        public Point3 Reveal {get;set;}

        /// <summary>
        /// Spawns the referenced object (Item, Actor, Adjective etc).  Target property dictates what to spawn the object into/onto. Must uniquely identify a single object blueprint
        /// </summary>
        public string Spawn {get;set;}

        /// <summary> 
        /// Applies a system to one or more targets e.g. inflict an injury, begin a spreading disease etc
        /// </summary>
        public ApplySystemBlueprint Apply {get;set;}
        
        /// <summary>
        /// Turns the blueprint into effect(s).  A blueprint can describe more than 1 effect e.g. if <see cref="Lua"/> is set but also <see cref="Kill"/> has a value
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEffect> Create()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                yield return new EffectCode(Lua);

            if(!string.IsNullOrWhiteSpace(Kill))
                yield return new KillEffect(Kill,Target);

            if (!string.IsNullOrWhiteSpace(Set))
                yield return new SetEffect(Set,Target);

            if(Reveal != null)
                yield return new RevealEffect(Reveal);

            if(!string.IsNullOrWhiteSpace(Spawn))
                yield return new SpawnEffect(Spawn,Target);

            if(Apply != null && (Apply.Name != null || Apply.Identifier != null))
                yield return new ApplyEffect(Apply,Target);
        }
    }
}
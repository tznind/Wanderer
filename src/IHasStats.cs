using System;
using System.Collections.Generic;
using System.Text;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Dialogues;
using Wanderer.Stats;

namespace Wanderer
{
    public interface IHasStats : IAreIdentical<IHasStats>
    {
        /// <summary>
        /// The guid for this type of object.  If the object was
        /// created from a blueprint then this guid is shared
        /// with other instances stamped out by the blueprint
        /// </summary>
        Guid? Identifier { get; set; }

        /// <summary>
        /// True to only ever generate one of this thing
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// Human readable name 
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// Black=0
        /// DarkBlue=1
        /// DarkGreen=2
        /// DarkCyan=3
        /// DarkRed=4
        /// DarkMagenta= 5
        /// DarkYellow=6
        /// Gray=7
        /// DarkGray=8
        /// Blue=9
        /// Green=10
        /// Cyan=11
        /// Red=12
        /// Magenta=13 
        /// Yellow=14
        /// White=15
        /// </summary>
        ConsoleColor Color { get; set; }

        /// <summary>
        /// What the thing does when you try to interact with it in a narrative manner e.g. talk, read
        /// </summary>
        DialogueInitiation Dialogue { get; set; }

        /// <summary>
        /// Human readable words that describe the current state of the object
        /// </summary>
        IAdjectiveCollection Adjectives { get; set; }

        /// <summary>
        /// The <see cref="IAction"/> that the object can undertake regardless of any child objects (gear, location etc.)
        /// </summary>
        IActionCollection BaseActions { get; set; }

        /// <summary>
        /// Stats (or modifiers) before applying any external child objects (gear, location etc.)
        /// </summary>
        StatsCollection BaseStats { get; set; }

        /// <summary>
        /// Determines how the object responds  before applying any external child objects (gear, location etc.)
        /// </summary>
        IBehaviourCollection BaseBehaviours { get; set; }

        /// <summary>
        /// Returns the <see cref="BaseStats"/> plus any modifiers for child objects (e.g. gear, <see cref="Adjectives"/> etc)
        /// </summary>
        /// <param name="forActor"></param>
        /// <returns></returns>
        StatsCollection GetFinalStats(IActor forActor);

        /// <summary>
        /// Returns the <see cref="BaseActions"/> plus any allowed by child objects, gear, <see cref="Adjectives"/> etc
        /// </summary>
        /// <param name="forActor"></param>
        /// <returns></returns>
        IActionCollection GetFinalActions(IActor forActor);


        /// <summary>
        /// Returns all behaviours the object including those granted by child objects (e.g. gear, adjectives etc) (super set of <see cref="BaseBehaviours"/> and any from gear, <see cref="IAdjective"/> etc)
        /// </summary>
        /// <param name="forActor"></param>
        /// <returns></returns>
        IBehaviourCollection GetFinalBehaviours(IActor forActor);

        /// <summary>
        /// Returns all the other things that the object has
        /// e.g.
        /// A Room has occupants and Adjectives
        /// An Actor has Items and Adjectives
        /// </summary>
        /// <returns></returns>
        IEnumerable<IHasStats> GetAllHaves();

        /// <summary>
        /// Returns all objects that you have that match <paramref name="name"/>
        /// </summary>
        /// <returns></returns>
        List<IHasStats> Get(string name);
        
        /// <summary>
        /// Returns all the other things that the object has where the <see cref="IHasStats.Identifier"/>
        /// is <paramref name="g"/>
        /// </summary>
        /// <returns></returns>
        List<IHasStats> Get(Guid? g);

        /// <summary>
        /// Returns true if the object or a child of it's has the uniquely identified
        /// other object (with <see cref="Identifier"/> equal to <paramref name="g"/>)
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        bool Has(Guid? g);

        /// <summary>
        /// Returns true if the object or a child of it's has an object named <paramref name="s"/>
        /// or of a Type named <paramref name="s"/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        bool Has(string s);

        /// <summary>
        /// Returns true if the supplied string matches the current object (but not children,
        /// use Has) for that.
        /// </summary>
        /// <param name="s"><see cref="Identifier"/> or <see cref="Name"/> to check for</param>
        /// <returns></returns>
        bool Is(string s);

        /// <summary>
        /// Returns true if the supplied <see cref="Identifier"/> matches the current object (but not children,
        /// use <see cref="Has(System.Nullable{System.Guid})"/> for that).
        /// </summary>
        /// <param name="s"><see cref="Identifier"/> or <see cref="Name"/> to check for</param>
        /// <returns></returns>
        bool Is(Guid? g);
    }
}

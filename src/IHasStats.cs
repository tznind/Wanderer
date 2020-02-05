using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Stats;

namespace StarshipWanderer
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
        /// Human readable name 
        /// </summary>
        string Name { get; set; }

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

    }
}

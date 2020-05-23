using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Compilation;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Blueprint that describes how to build one or more <see cref="ICondition"/>
    /// </summary>
    public class ConditionBlueprint
    {
        /// <summary>
        /// Lua code that returns true or false
        /// </summary>
        public string Lua { get; set; }

        /// <summary>
        /// Pass a Guid or Name of something they might have, if they have it then the condition is met
        /// </summary>
        public string Has { get; set; }

        /// <summary>
        /// Pass a Guid or Name, condition is true if the object is it.  This does not include things they have e.g. items, adjectives etc (see <see cref="Has"/> for that)
        /// </summary>
        public string Is {get;set;}

        /// <summary>
        /// Pass a Guid or Name of something.  As long as they don't have it this condition is true
        /// </summary>
        public string HasNot {get;set;}

        /// <summary>
        /// Pass a Guid or Name, condition is true as long as the object is NOT it.  This does not include things they have e.g. items, adjectives etc  (see <see cref="Has"/> for that)
        /// </summary>
        public string IsNot {get;set;}

        /// <summary>
        /// Pass a Guid or Name of something that might be in the room they in.  Condition is true as long as the room or anyone in it (or their items) has this
        /// </summary>
        public string RoomHas { get; set; }

        /// <summary>
        /// Pass a Guid or Name of something that might be in the room they in.  Condition is true as long as the room or anyone in it (or their items) DOES NOT have this
        /// </summary>
        public string RoomHasNot { get; set; }
        

        /// <summary>
        /// Pass a Guid or Name of a room.  Condition is true if the room fits it.  This does not include adjectives or occupants, see <see cref="RoomHas"/> for that.
        /// </summary>
        public string RoomIs { get; set; }

        /// <summary>
        /// Pass a Guid or Name of a room.  Condition is true as long as the room is NOT it.  This does not include adjectives or occupants, see <see cref="RoomHasNot"/> for that.
        /// </summary>
        public string RoomIsNot { get; set; }

        /// <summary>
        /// Arithmetic expression for a required stat they must have e.g. "Fight > 50"
        /// </summary>
        public string Stat {get;set;}

        /// <summary>
        /// Arithmetic expression for a required stat the secondary object must have e.g. "Fight > 50".  Secondary object in dialogue is who you are talking to, for an action it is the target of the action etc.
        /// </summary>
        public string StatRecipient { get; set; }

        /// <summary>
        /// Arithmetic expression for a required custom variable e.g. "MyCounter > 50" (See <see cref="IHasStats.V"/>)
        /// </summary>
        public string Variable {get;set;}

        
        /// <summary>
        /// Arithmetic expression for a required custom variable the secondary object must have e.g. "MyCounter > 50".  Secondary object in dialogue is who you are talking to, for an action it is the target of the action etc.
        /// </summary>
        public string VariableRecipient { get; set; }
        
        /// <summary>
        /// Creates one <see cref="ICondition"/> for each configured blueprint option e.g. <see cref="Lua"/> creates a <see cref="ConditionCode"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICondition> Create()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                yield return new ConditionCode(Lua);

            if (!string.IsNullOrWhiteSpace(Has))
                yield return new HasCondition(Has);

            if (!string.IsNullOrWhiteSpace(HasNot))
                yield return new HasCondition(HasNot) { InvertLogic = true};

            if (!string.IsNullOrWhiteSpace(Is))
                yield return new HasCondition(Is){UseIs = true};

            if (!string.IsNullOrWhiteSpace(IsNot))
                yield return new HasCondition(IsNot){InvertLogic = true, UseIs = true};

            if(!string.IsNullOrWhiteSpace(RoomHas))
                yield return new HasCondition(RoomHas){CheckRoom = true};
            
            if (!string.IsNullOrWhiteSpace(RoomHasNot))
                yield return new HasCondition(RoomHasNot) { CheckRoom = true,InvertLogic = true};

            if(!string.IsNullOrWhiteSpace(RoomIs))
                yield return new HasCondition(RoomIs){CheckRoom = true, UseIs = true};
            
            if (!string.IsNullOrWhiteSpace(RoomIsNot))
                yield return new HasCondition(RoomIsNot) { CheckRoom = true,InvertLogic = true, UseIs = true};

            if(!string.IsNullOrWhiteSpace(Stat))
                yield return new StatCondition(Stat);

            if(!string.IsNullOrWhiteSpace(StatRecipient))
                yield return new StatCondition(StatRecipient){RecipientOnly = true};
            
            if(!string.IsNullOrWhiteSpace(Variable))
                yield return new VariableCondition(Variable);

            if(!string.IsNullOrWhiteSpace(VariableRecipient))
                yield return new VariableCondition(VariableRecipient){RecipientOnly = true};
            
                
        }
    }
}
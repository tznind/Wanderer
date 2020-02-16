using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Places;
using Wanderer.Relationships;

namespace Wanderer.Systems.Validation
{
    public class WorldValidator
    {
        public StringBuilder Errors { get; set; } = new StringBuilder();
        public bool IncludeStackTraces { get; set; }
        public StringBuilder Warnings { get; set; } = new StringBuilder();

        ///<summary>
        /// Avoid circular checking and hence stack overflows
        ///</summary>
        List<Guid> _alreadyValidated = new List<Guid>();

        public void Validate(WorldFactory worldFactory)
        {
            IWorld w;
            try
            {
                w = worldFactory.Create();
            }
            catch (Exception e)
            {
                AddError("Error Creating World", e);
                return;
            }

            Validate(w);
        }

        public void Validate(IWorld world)
        {
            Validate(world,world.RoomFactory, "World RoomFactory");

            foreach (IFaction faction in world.Factions)
            {
                Validate(world,faction.RoomFactory,$"Faction <{faction}> RoomFactory");
            }
        }

        protected virtual Npc GetTestActor(IPlace room)
        {
            return new Npc("test actor",room);
        }

        private void AddError(string msg, Exception exception)
        {
            Errors.AppendLine(msg);
            Errors.AppendLine(IncludeStackTraces ? exception.ToString() : Flatten(exception));
        }
        private void AddError(string msg)
        {
            Errors.AppendLine(msg);
        }


        private void AddWarning(string msg, Exception exception)
        {
            Warnings.AppendLine(msg);
            Warnings.AppendLine(IncludeStackTraces ? Flatten(exception) : exception.Message);
        }

        private string Flatten(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            while(ex != null)
            {
                sb.AppendLine(ex.Message);
                ex = ex.InnerException;
            }

            return sb.ToString();
        }
        public void Validate(IWorld world,IRoomFactory roomFactory,string title)
        {

            foreach (var blue in roomFactory.Blueprints)
            {
                try
                {
                    var room = roomFactory.Create(world, blue);


                    foreach (var item in room.Items.ToArray())
                        Validate(world,item,room);
                    
                    foreach (var actor in room.Actors.ToArray())
                    {
                        if (actor.Dialogue != null)
                            Validate(world,actor, actor.Dialogue,room);
                        
                        foreach (var item in actor.Items)
                            Validate(world, item,room);
                    }
                }
                catch (Exception e)
                {
                    AddError($"Error creating RoomBlueprint for RoomFactory '{title}'.  Error was in {blue.Identifier?.ToString() ?? "Unamed Blueprint"}"  ,e);
                }
            }
        }

        public void Validate(IWorld world, IItem item, IPlace room)
        {
            if (item.Dialogue != null)
                Validate(world, item, item.Dialogue,room);
            
            try
            {
                item.RequirementsMet(GetTestActor(room));
            }
            catch (Exception e)
            {
                AddWarning($"Error testing conditions of item '{item}' in room '{room.Name}' with test actor", e);
            }
        }



        public void Validate(IWorld world, IHasStats recipient, DialogueInitiation dialogue, IPlace room)
        {
            if (!dialogue.Next.HasValue)
                return;

            if(_alreadyValidated.Contains(dialogue.Next.Value))
                return;
            else
                _alreadyValidated.Add(dialogue.Next.Value);

            var d = world.Dialogue.GetDialogue(dialogue.Next);

            if (d == null)
            {
                AddError($"Could not find Dialogue '{dialogue.Next}'");
                return;
            }

            if(d.Body == null || d.Body.Length == 0 || d.Body.All(b=>string.IsNullOrWhiteSpace(b.Text)))
                AddError($"Dialogue '{d.Identifier}' has no Body Text");
                
            foreach (ICondition<SystemArgs> condition in d.Require)
            {
                try
                {
                    condition.IsMet(new SystemArgs(world,null, 0, GetTestActor(room), recipient, Guid.Empty));
                }
                catch (Exception e)
                {
                    AddWarning($"Error testing dialogue condition on '{d}' for test actor interacting with '{recipient}'",e);
                }
            }

            foreach(var option in d.Options)
                Validate(world,recipient,dialogue,room,d,option);

        }

        public void Validate(IWorld world, IHasStats recipient, DialogueInitiation initiation, IPlace room,DialogueNode dialogue, DialogueOption option)
        {
            if(string.IsNullOrWhiteSpace(option.Text))
                AddError($"A Dialogue Option of Dialogue '{dialogue.Identifier}' has no Text");
            
            foreach (IEffect effect in option.Effect)
            {
                try
                {
                    effect.Apply(new SystemArgs(world,null, 0, GetTestActor(room), recipient, Guid.Empty));
                }
                catch (Exception e)
                {
                    AddWarning($"Error testing EffectCode of Option '{option.Text}' of Dialogue '{dialogue.Identifier}' for test actor interacting with '{recipient}'",e);
                }
            }

            if(option.Destination != null)
            {
                initiation.Next = option.Destination;
                Validate(world,recipient,initiation,room);
            }
        }

    }
}

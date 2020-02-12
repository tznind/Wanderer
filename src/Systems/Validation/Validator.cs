using System;
using System.Linq;
using System.Text;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Places;
using Wanderer.Relationships;
using Wanderer.Systems;

namespace Wanderer.Validation
{
    public class WorldValidator
    {
        public StringBuilder Errors { get; set; } = new StringBuilder();
        public bool IncludeStackTraces { get; set; }
        public StringBuilder Warnings { get; set; } = new StringBuilder();

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
            Errors.AppendLine(IncludeStackTraces ? exception.ToString() : exception.Message);
        }
        private void AddWarning(string msg, Exception exception)
        {
            Warnings.AppendLine(msg);
            Warnings.AppendLine(IncludeStackTraces ? exception.ToString() : exception.Message);
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

            var d = world.Dialogue.GetDialogue(dialogue.Next);

            if (d == null)
            {
                Errors.AppendLine($"Could not find Dialogue '{dialogue.Next}'");
                return;
            }

            foreach (ICondition<SystemArgs> condition in d.Require)
            {
                try
                {
                    condition.IsMet(new SystemArgs(null, 0, GetTestActor(room), recipient, Guid.Empty));
                }
                catch (Exception e)
                {
                    AddWarning($"Error testing dialogue condition on '{d}' for test actor interacting with '{recipient}'",e);
                }
            }

        }
    }
}

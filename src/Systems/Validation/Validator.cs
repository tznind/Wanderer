using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Plans;
using Wanderer.Relationships;

namespace Wanderer.Systems.Validation
{

    

    public class WorldValidator
    {
        public int ErrorCount {get;set;} = 0;
        public int WarningCount {get;set;} = 0;

        public StringBuilder Errors { get; set; } = new StringBuilder();
        public bool IncludeStackTraces { get; set; }
        public StringBuilder Warnings { get; set; } = new StringBuilder();

        ///<summary>
        /// Avoid circular checking and hence stack overflows
        ///</summary>
        List<Guid> _alreadyValidated = new List<Guid>();

        private IUserinterface _ui = new ValidatorUI();

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
        }

        protected virtual Npc GetTestActor(IRoom room)
        {
            return new Npc("test actor",room);
        }

        private void AddError(string msg, Exception exception)
        {
            Errors.AppendLine(msg);
            Errors.AppendLine(IncludeStackTraces ? exception.ToString() : Flatten(exception));

            ErrorCount++;
        }
        private void AddError(string msg)
        {
            Errors.AppendLine(msg);
        }


        private void AddWarning(string msg, Exception exception)
        {
            Warnings.AppendLine(msg);
            Warnings.AppendLine(IncludeStackTraces ?  exception.ToString() : Flatten(exception));

            WarningCount++;
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
                    Validate(world,room);
                }
                catch (Exception e)
                {
                    AddError($"Error creating RoomBlueprint for RoomFactory '{title}'.  Error was in {blue.Identifier?.ToString() ?? "Unamed Blueprint"}"  ,e);
                }
            }
        }

        public void Validate(IWorld world, IRoom room)
        {
            foreach (var item in room.Items.ToArray())
                Validate(world,item,room);
                    
            if(room.Dialogue != null)
                Validate(world,room,room.Dialogue,room);

            foreach (var actor in room.Actors.ToArray())
            {
                if (actor.Dialogue != null)
                    Validate(world,actor, actor.Dialogue,room);
                        
                foreach (var item in actor.Items)
                    Validate(world, item,room);

                foreach(var plan in world.PlanningSystem.Plans)
                    Validate(world,plan,actor);
            }

            Console.WriteLine($"Validated {room}");
        }

        public void Validate(IWorld world, Plan plan, IActor actor)
        {
            foreach(var condition in plan.Condition)
            {
                try
                {
                    condition.IsMet(world,new SystemArgs(world,_ui,0,null,actor,Guid.Empty));
                }
                catch(Exception e)
                {
                    AddWarning($"Failed to validate Condition {condition} on Plan '{plan}' when using '{actor}' in '{actor.CurrentLocation}'",e);
                }
            }
            

            try
            {
                if(plan.Do == null )
                    AddError($"Plan '{plan}' has no DoFrame");
                else
                {
                    var f = plan.Do.GetFrame(new SystemArgs(world,_ui,0,null,actor,Guid.Empty));

                    if(f == null)
                        throw new Exception("Script returned a null Frame");
                }
            }
            catch(Exception e)
            {
                AddWarning($"Failed to validate DoFrame of Plan '{plan}' when using '{actor}' in '{actor.CurrentLocation}'",e);
            }
        }

        public void Validate(IWorld world, IItem item, IRoom room)
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



        public void Validate(IWorld world, IHasStats recipient, DialogueInitiation dialogue, IRoom room)
        {
            if (dialogue.Next.HasValue)
            {
                var d = world.Dialogue.GetDialogue(dialogue.Next);
                
                if (d == null)
                    AddError($"Could not find Dialogue '{dialogue.Next}'");
                else
                    Validate(world, recipient, dialogue,d, room);
            }
            
            if (dialogue.Banter != null)
                foreach (var guid in dialogue.Banter)
                {
                    var d = world.Dialogue.GetDialogue(guid);

                    if (d == null)
                        AddError($"Could not find Banter Dialogue '{dialogue.Next}'");
                    else
                        Validate(world, recipient, dialogue,d, room);
                }
        }


        public void Validate(IWorld world, IHasStats recipient,DialogueInitiation dialogue, DialogueNode node, IRoom room)
        {
            if(_alreadyValidated.Contains(node.Identifier))
                return;
            
            _alreadyValidated.Add(node.Identifier);


            if(node.Body == null || node.Body.Count == 0 || node.Body.All(b=>string.IsNullOrWhiteSpace(b.Text)))
                AddError($"Dialogue '{node.Identifier}' has no Body Text");
                
            foreach (ICondition<SystemArgs> condition in node.Require)
            {
                try
                {
                    condition.IsMet(world,new SystemArgs(world,_ui, 0, GetTestActor(room), recipient, Guid.Empty));
                }
                catch (Exception e)
                {
                    AddWarning($"Error testing dialogue condition on '{node}' for test actor interacting with '{recipient}'",e);
                }
            }

            if(node.Body != null)
                foreach (ICondition<SystemArgs> condition in node.Body.SelectMany(b => b.Condition))
                {
                    try
                    {
                        condition.IsMet(world,new SystemArgs(world,_ui, 0, GetTestActor(room), recipient, Guid.Empty));
                    }
                    catch (Exception e)
                    {
                        AddWarning($"Error testing dialogue BodyText Condition on '{node}' for test actor interacting with '{recipient}'",e);
                    }
                }


            foreach(var option in node.Options)
                Validate(world,recipient,dialogue,room,node,option);

        }
        public void Validate(IWorld world, IHasStats recipient, DialogueInitiation initiation, IRoom room,DialogueNode dialogue, DialogueOption option)
        {
            if(string.IsNullOrWhiteSpace(option.Text))
                AddError($"A Dialogue Option of Dialogue '{dialogue.Identifier}' has no Text");
            
            foreach (IEffect effect in option.Effect)
            {
                try
                {
                    effect.Apply(new SystemArgs(world,_ui, 0, GetTestActor(room), recipient, Guid.Empty));
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
        
        private class ValidatorUI : IUserinterface
        {
            public ValidatorUI()
            {
                Log.Register();
            }
            public EventLog Log { get; } = new EventLog();

            public void ShowStats(IHasStats of)
            {

            }

            public bool GetChoice<T>(string title, string body, out T chosen, params T[] options)
            {
                chosen = options.FirstOrDefault();
                return true;
            }

            public void Refresh()
            {
                
            }

            public void ShowMessage(string title, string body)
            {
                
            }

            public void ShowMessage(string title, LogEntry showThenLog)
            {
                
            }
        }
    }
}

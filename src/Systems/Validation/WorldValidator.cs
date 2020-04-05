using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Plans;
using Wanderer.Relationships;
using Action = Wanderer.Actions.Action;

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
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        
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

            _log.Info("-----------------------------");
            _log.Info("Validating World");
            _log.Info("-----------------------------");
            Validate(world,world.RoomFactory, "World RoomFactory");
            Validate(world,world.ItemFactory, "World ItemFactory");
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
            ErrorCount++;
        }


        private void AddWarning(string msg, Exception exception)
        {
            Warnings.AppendLine(msg);
            Warnings.AppendLine(IncludeStackTraces ?  exception.ToString() : Flatten(exception));

            WarningCount++;
        }
        private void AddWarning(string msg)
        {
            Warnings.AppendLine(msg);
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
                    world.Map[new Point3(0,0,0)] = room;
                    world.Player.CurrentLocation = room;
                    ValidateRoom(world,room);
                }
                catch (Exception e)
                {
                    AddError($"Error creating RoomBlueprint for RoomFactory '{title}'.  Error was in {blue.Identifier?.ToString() ?? "Unamed Blueprint"}"  ,e);
                }
            }
        }
        public void Validate(IWorld world,IItemFactory itemFactory,string title)
        {

            foreach (var blue in itemFactory.Blueprints)
            {
                try
                {
                    var item = itemFactory.Create(world, blue);
                    var room = new Room("Test Room",world,'t');
                    world.Map[new Point3(0,0,0)] = room;
                    world.Player.CurrentLocation = room;
                    ValidateItem(world,item, room);
                }
                catch (Exception e)
                {
                    AddError($"Error creating ItemBlueprint for ItemFactory '{title}'.  Error was in {blue}"  ,e);
                }
            }
        }

        public void ValidateRoom(IWorld world, IRoom room)
        {
            if(room.Unique && room.Identifier == null)
                AddError("Unique rooms must have an Identifier");

            foreach (var item in room.Items.ToArray())
                ValidateItem(world,item,room);
                    
            if(room.Dialogue != null)
                ValidateDialogue(world,room,room.Dialogue,room);

            foreach (var actor in room.Actors.ToArray())
            {
                if (actor.Dialogue != null)
                    ValidateDialogue(world,actor, actor.Dialogue,room);
                    
                foreach (var behaviour in actor.BaseBehaviours)
                    ValidateBehaviour(world,actor,behaviour,room);
                
                foreach (var item in actor.Items)
                    ValidateItem(world, item,room);

                foreach(var plan in world.PlanningSystem.Plans)
                    Validate(world,plan,actor);
            }

            _log?.Info($"Validated {room}");
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

        public void ValidateItem(IWorld world, IItem item, IRoom room)
        {
            if (item.Dialogue != null)
                ValidateDialogue(world, item, item.Dialogue,room);

            //if the item takes up slots
            if (item.Slot != null && !string.IsNullOrWhiteSpace(item.Slot.Name))
            {
                //make sure somebody in the world can use it
                var slots = GetAllSlots(world);

                if(!slots.Contains(item.Slot.Name))
                    AddWarning($"Item {item} lists Slot named {item.Slot.Name} but no Actors or Default slots are listed with that name (Slots seen were '{string.Join(',',slots)}')");
            }

            foreach (var behaviour in item.BaseBehaviours) 
                ValidateBehaviour(world, item, behaviour, room);

            foreach (var action in item.BaseActions) 
                ValidateAction(world, item, action, room);

            try
            {
                item.RequirementsMet(GetTestActor(room));
            }
            catch (Exception e)
            {
                AddWarning($"Error testing conditions of itemFactory '{item}' in room '{room.Name}' with test actor", e);
            }
        }

        public void ValidateBehaviour(IWorld world, IHasStats owner, IBehaviour behaviour, IRoom room)
        {
            var actor = owner as IActor ?? GetTestActor(room);

            var testAction = new Action(actor) { Name = "Test Action"};
            var stack = new ActionStack();
            
            try
            {
                behaviour.OnPush(world,_ui,stack,new Frame(actor,testAction,0));
            }
            catch (Exception e)
            {
                AddWarning($"Error testing OnPush of Behaviour {behaviour} of on '{owner}' in room '{room.Name}' with test actor '{actor}'", e);
            }
            
            try
            {
                behaviour.OnPop(world,_ui,stack,new Frame(actor,testAction,0));
            }
            catch (Exception e)
            {
                AddWarning($"Error testing OnPop of Behaviour {behaviour} of on '{owner}' in room '{room.Name}' with test actor '{actor}'", e);
            }
            
            try
            {
                behaviour.OnRoundEnding(world,_ui,Guid.NewGuid());
            }
            catch (Exception e)
            {
                AddWarning($"Error testing OnRoundEnding of Behaviour {behaviour} of on '{owner}' in room '{room.Name}' with test actor '{actor}'", e);
            }
        }
        public void ValidateAction(IWorld world, IHasStats owner, IAction action, IRoom room)
        {
            var actor = owner as IActor ?? GetTestActor(room);
            
            foreach (var effect in action.Effect)
            {
                try
                {
                    effect.Apply(new ActionSystemArgs(action,world,_ui, 0, GetTestActor(room), actor, Guid.Empty));
                }
                catch (Exception e)
                {
                    AddWarning($"Error testing Effect Code of Action '{action}' of '{owner}'.  Bad code was:{effect}",e);
                }
            }
        }
        private List<string> GetAllSlots(IWorld world)
        {
            List<string> slots = new List<string>();

            slots.AddRange(world.ActorFactory.DefaultSlots.Select(s => s.Key));
            slots.AddRange(world.ActorFactory.Blueprints.Where(b => b.Slots != null && b.Slots.Any()).SelectMany(b => b.Slots.Keys));

            return slots.Distinct().ToList();
        }

        public void ValidateDialogue(IWorld world, IHasStats recipient, DialogueInitiation dialogue, IRoom room)
        {
            if (dialogue.Next.HasValue)
            {
                var d = world.Dialogue.GetDialogue(dialogue.Next);
                
                if (d == null)
                    AddError($"Could not find Dialogue '{dialogue.Next}'");
                else
                    ValidateDialogue(world, recipient, dialogue,d, room);
            }
            
            if (dialogue.Banter != null)
                foreach (var guid in dialogue.Banter)
                {
                    var d = world.Dialogue.GetDialogue(guid);

                    if (d == null)
                        AddError($"Could not find Banter Dialogue '{dialogue.Next}'");
                    else
                        ValidateDialogue(world, recipient, dialogue,d, room);
                }
        }


        public void ValidateDialogue(IWorld world, IHasStats recipient,DialogueInitiation dialogue, DialogueNode node, IRoom room)
        {
            if(_alreadyValidated.Contains(node.Identifier))
                return;
            
            _alreadyValidated.Add(node.Identifier);


            if(node.Body == null || node.Body.Count == 0 || node.Body.All(b=>string.IsNullOrWhiteSpace(b.Text)))
                AddError($"Dialogue '{node.Identifier}' has no Body Text");
                
            foreach (ICondition<SystemArgs> condition in node.Condition)
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
                    AddWarning($"Error testing EffectCode of Option '{option.Text}' of Dialogue '{dialogue.Identifier}' for test actor interacting with '{recipient}'  Bad code was:{effect}",e);
                }
            }

            foreach (ICondition<SystemArgs> condition in option.Condition)
            {
                try
                {
                    condition.IsMet(world,new SystemArgs(world,_ui, 0, GetTestActor(room), recipient, Guid.Empty));
                }
                catch (Exception e)
                {
                    AddWarning($"Error testing Condition Code of Option '{option.Text}' of Dialogue '{dialogue.Identifier}' for test actor interacting with '{recipient}'.  Bad code was:{condition}",e);
                }
            }

            if(option.Destination != null)
            {
                initiation.Next = option.Destination;
                ValidateDialogue(world,recipient,initiation,room);
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


            public void ShowMessage(string title, string body)
            {
                
            }
        }
    }
}

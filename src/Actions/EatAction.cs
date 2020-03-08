using System;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    public class EatAction : Action
    {

        public override char HotKey => 'e';

        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            stack.Push(new Frame(actor,this,5));
        }

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var hunger = frame.PerformedBy.Adjectives.OfType<IInjured>()
                //TODO: this is a reference to Hunger.yaml once actions are yamled too then this should move there
                .FirstOrDefault(i => i.InjurySystem.Identifier == new Guid("89c18233-5250-4445-8799-faa9a888fb7f"));

            hunger?.Heal(ui,stack.Round);

            frame.GetActionOwner();

            ui.Log.Info(new LogEntry($"{frame.PerformedBy} ate {frame.GetActionOwner()?.Name}",stack.Round,frame.PerformedBy));
        }

        public override bool HasTargets(IActor performer)
        {
            return true;
        }
    }
}
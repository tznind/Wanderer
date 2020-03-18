using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Items;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    public class EatAction : Action
    {
        
        private EatAction():base(null)
        {
        }

        public EatAction(IHasStats owner):base(owner)
        {
            Owner = owner;
        }

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

        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            if (Owner != null)
                yield return Owner;
        }
    }
}
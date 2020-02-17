using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    public class EatAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            stack.Push(new Frame(actor,this,5));
        }

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var hunger = frame.PerformedBy.Adjectives.OfType<IInjured>()
                .FirstOrDefault(i => i.InjurySystem is HungerInjurySystem);

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
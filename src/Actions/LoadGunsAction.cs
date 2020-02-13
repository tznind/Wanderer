using System.Text;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Stats;

namespace Wanderer.Actions
{
    public class LoadGunsAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            stack.Push(new Frame(actor,this,0));
        }

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var narrative = new Narrative(frame.PerformedBy,"Load Guns","You spend several hours pushing overloaded gun carriages in the sweat and smoke filled confines of the loading bay.",null,stack.Round);

            if (frame.PerformedBy.BaseStats[Stat.Loyalty] > 0 && frame.PerformedBy.BaseStats[Stat.Loyalty] < 30)
            {
                frame.PerformedBy.BaseStats[Stat.Loyalty]++;
                narrative.Changed("The Emperor's grace fills your heart",Stat.Loyalty, 1);
            }

            if (frame.PerformedBy.BaseStats[Stat.Fight] > 20)
            {
                frame.PerformedBy.BaseStats[Stat.Fight]--;
                narrative.Changed("Aches and pains sap your strength",Stat.Fight, -1);
            }

            narrative.Show(ui,false);
        }

        public override bool HasTargets(IActor performer)
        {
            return true;
        }
    }
}
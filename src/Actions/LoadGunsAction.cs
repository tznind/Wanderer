using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;
using Terminal.Gui;

namespace StarshipWanderer.Actions
{
    internal class LoadGunsAction : Action
    {
        public LoadGunsAction(IWorld world,IActor performedBy):base(world,performedBy)
        {
            
        }
        
        public override void Pop(IUserinterface ui, ActionStack stack)
        {
            var narrative = new Narrative(PerformedBy,"Load Guns","You spend several hours pushing overloaded gun carriages in the sweat and smoke filled confines of the loading bay.");

            if (PerformedBy.BaseStats[Stat.Loyalty] > 0 && PerformedBy.BaseStats[Stat.Loyalty] < 30)
            {
                PerformedBy.BaseStats[Stat.Loyalty]++;
                narrative.Changed("The Emperor's grace fills your heart",Stat.Loyalty, 1);
            }

            if (PerformedBy.BaseStats[Stat.Fight] > 20)
            {
                PerformedBy.BaseStats[Stat.Fight]--;   
                narrative.Changed("Aches and pains sap your strength",Stat.Fight, -1);
            }

            narrative.Show(ui,false);
        }
    }
}
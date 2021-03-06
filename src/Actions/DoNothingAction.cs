﻿using System.Collections.Generic;
using System.Text;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Stats;

namespace Wanderer.Actions
{
    public class DoNothingAction : Action
    {
        public DoNothingAction(IHasStats owner):base(owner)
        {
            HotKey = 'n';
        }

        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            stack.Push(new Frame(actor,this,0));
        }

        protected override void PopImpl(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var narrative = new Narrative(frame.PerformedBy,"Load Guns","You spend several hours pushing overloaded gun carriages in the sweat and smoke filled confines of the loading bay.",null,stack.Round);

            if (frame.PerformedBy.BaseStats[Stat.Coerce] > 0 && frame.PerformedBy.BaseStats[Stat.Coerce] < 30)
            {
                frame.PerformedBy.BaseStats[Stat.Coerce]++;
                narrative.Changed("The Emperor's grace fills your heart",Stat.Coerce, 1);
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

        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            if (Owner != null)
                yield return Owner;
        }
    }
}
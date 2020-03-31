using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanderer.Actors;

namespace Wanderer.Actions
{
    public class InspectAction : Action
    {
        public InspectAction(IHasStats owner) : base(owner)
        {
            HotKey = 'i';
        }
   
        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            if(Owner != null)
                ui.ShowStats(Owner);
        }

        public override bool HasTargets(IActor performer)
        {
            return Owner != null;
        }

        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            if (Owner != null)
                yield return Owner;
        }
    }
}

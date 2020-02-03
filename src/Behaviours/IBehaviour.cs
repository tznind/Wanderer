using System;
using System.Text;
using StarshipWanderer.Actions;

namespace StarshipWanderer.Behaviours
{
    public interface IBehaviour : IAreIdentical<IBehaviour>
    {
        IHasStats Owner { get; set; }

        void OnPush(IUserinterface ui, ActionStack stack,Frame frame);

        void OnRoundEnding(IUserinterface ui,Guid round);
    }
}

using System;
using System.Text;
using StarshipWanderer.Actions;

namespace StarshipWanderer.Behaviours
{
    public interface IBehaviour : IAreIdentical<IBehaviour>
    {
        void OnPush(IUserinterface ui, ActionStack stack,Frame frame);

        void OnRoundEnding(IUserinterface ui,Guid round);
    }
}

using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    public class Relationship : IRelationship
    {
        public IActor Observer { get; set; }
        public IActor Observed { get; set; }
        public int Attitude { get; set; }

        public Relationship(IActor observer, IActor observed)
        {
            Observer = observer;
            Observed = observed;
            var observerStats = observer.GetFinalStats();
            var observedStats = observed.GetFinalStats();

        }

        public void HandleActorDeath(Npc npc)
        {
            if (Observer == npc || Observed == npc) 
                Observer.CurrentLocation.World.Relationships.Remove(this);
        }

        public bool AppliesTo(IActor observer, IActor observed)
        {
            return observer == Observer && observed == Observed;
        }
    }
}

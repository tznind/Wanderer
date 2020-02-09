using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    public abstract class FactionRelationship : IFactionRelationship
    {
        public IFaction HostFaction { get; set; }
        
        public double Attitude { get; set; }


        /// <summary>
        /// Normally the death of members does not affect <see cref="FactionRelationship"/>.
        /// Override if it should.
        /// </summary>
        /// <param name="npc"></param>
        public virtual void HandleActorDeath(Npc npc)
        {

        }

        public abstract bool AppliesTo(IActor observer, IActor observed);

        public abstract bool AppliesTo(IFaction other);

        protected FactionRelationship(IFaction hostFaction,double attitude)
        {
            HostFaction = hostFaction;
            Attitude = attitude;
        }

    }
}
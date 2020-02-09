using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    /// <summary>
    /// Models a relationship between members of the same <see cref="IFaction"/>
    /// </summary>
    public class IntraFactionRelationship : FactionRelationship
    {
        
        public IntraFactionRelationship(IFaction hostFaction,double attitude) : base(hostFaction,attitude)
        {
        }
        
        public override bool AppliesTo(IActor observer, IActor observed)
        {
            return observer.FactionMembership.Contains(HostFaction) && observed.FactionMembership.Contains(HostFaction);
        }

        public override bool AppliesTo(IFaction other)
        {
            return other == HostFaction;
        }
    }
}
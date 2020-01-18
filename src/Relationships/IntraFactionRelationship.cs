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
            return observed.FactionMembership.Contains(HostFaction) && observed.FactionMembership.Contains(HostFaction);
        }
    }
}
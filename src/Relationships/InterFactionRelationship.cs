using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    /// <summary>
    /// Models a relationship between all members of one faction and all members
    /// of another faction
    /// </summary>
    public class InterFactionRelationship : FactionRelationship
    {
        public IFaction ObservedFaction { get; set; }

        public InterFactionRelationship(IFaction hostFaction, IFaction observedFactionFaction, double attitude) : base(hostFaction,attitude)
        {
            ObservedFaction = observedFactionFaction;
        }


        public override bool AppliesTo(IActor observer, IActor observed)
        {
            return observer.FactionMembership.Contains(HostFaction)
                   && observed.FactionMembership.Contains(ObservedFaction);
        }
    }
}
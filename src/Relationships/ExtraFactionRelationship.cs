using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    /// <summary>
    /// Models a relationship between all members of one faction against
    /// anyone not a member of their faction.
    /// </summary>
    public class ExtraFactionRelationship : FactionRelationship
    {
        public ExtraFactionRelationship(Faction hostFaction, double attitude) : base(hostFaction, attitude)
        {
        }

        public override bool AppliesTo(IActor observer, IActor observed)
        {
            //if you are in the faction and they are not
            return observer.FactionMembership.Contains(HostFaction) && !observed.FactionMembership.Contains(HostFaction);
        }

    }
}
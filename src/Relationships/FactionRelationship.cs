using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    /// <summary>
    /// Models a relationship between members of the same <see cref="IFaction"/>
    /// </summary>
    public class FactionRelationship : IRelationship
    {
        public IFaction Faction { get; set; }
        
        public double Attitude { get; set; }

        public FactionRelationship(IFaction faction)
        {
            Faction = faction;
        }

        public void HandleActorDeath(Npc npc)
        {
            
        }

        public bool AppliesTo(IActor observer, IActor observed)
        {
            return observed.FactionMembership.Contains(Faction) && observed.FactionMembership.Contains(Faction);
        }
    }
}
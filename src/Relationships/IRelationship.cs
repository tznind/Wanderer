using StarshipWanderer.Actors;

namespace StarshipWanderer.Relationships
{
    public interface IRelationship
    {
        /// <summary>
        /// Adjusts or deletes the current relationship based on the fact that
        /// <paramref name="npc"/> has just died
        /// </summary>
        /// <param name="npc"></param>
        void HandleActorDeath(Npc npc);
    }
}
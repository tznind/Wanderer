using Wanderer.Actors;

namespace Wanderer.Relationships
{
    public interface IRelationship
    {
        /// <summary>
        /// How strong the relationship is, positive is good relationship, negative is hate
        /// </summary>
        double Attitude { get; set; }

        /// <summary>
        /// Adjusts or deletes the current relationship based on the fact that
        /// <paramref name="npc"/> has just died
        /// </summary>
        /// <param name="npc"></param>
        void HandleActorDeath(Npc npc);

        /// <summary>
        /// Returns true if this relationship comes into play when <paramref name="observer"/>
        /// considers performing an action on <paramref name="observed"/>
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="observed"></param>
        /// <returns></returns>
        bool AppliesTo(IActor observer, IActor observed);
    }
}
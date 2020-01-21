using StarshipWanderer.Adjectives;

namespace StarshipWanderer.Factories.Blueprints
{
    public class ActorBlueprint
    {
        /// <summary>
        /// Explicit name for this actor type e.g. Centipede otherwise leave null to generate
        /// a random name from the faction <see cref="NameFactory"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of <see cref="IAdjective"/> type names (which must be supported by
        /// <see cref="IAdjectiveFactory"/>) from which to pick at random when creating
        /// this Npc
        /// </summary>
        public AdjectiveBlueprint[] Adjectives { get;set; }

    }
}
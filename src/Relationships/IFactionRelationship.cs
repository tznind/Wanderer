namespace Wanderer.Relationships
{
    public interface IFactionRelationship : IRelationship
    {
        /// <summary>
        /// The <see cref="IFaction"/> for which the relationship applies (all members
        /// will normally consider this relationship when forming opinions)
        /// </summary>
        IFaction HostFaction { get; set; }

        /// <summary>
        /// True if the relationship applies when the <see cref="HostFaction"/> considers this
        /// <paramref name="other"/> faction.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool AppliesTo(IFaction other);
    }
}
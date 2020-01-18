namespace StarshipWanderer.Relationships
{
    public interface IFactionRelationship : IRelationship
    {
        /// <summary>
        /// The <see cref="IFaction"/> for which the relationship applies (all members
        /// will normally consider this relationship when forming opinions)
        /// </summary>
        IFaction HostFaction { get; set; }
    }
}
namespace StarshipWanderer.Relationships
{
    public enum FactionRole
    {
        None = 0,

        /// <summary>
        /// Regular factions that just want to mind their own business
        /// </summary>
        Civilian,

        /// <summary>
        /// Authorities and government etc
        /// </summary>
        Establishment,

        /// <summary>
        /// Animals and plants, usually hostile.  Probably shouldn't allow signing up
        /// </summary>
        Wildlife,

        /// <summary>
        /// Factions that start out or are normally considered the 'bad guys' e.g. cults.
        /// </summary>
        Opposition
    }
}
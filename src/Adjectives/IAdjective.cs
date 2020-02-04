using System.Collections.Generic;

namespace StarshipWanderer.Adjectives
{
    public interface IAdjective : IHasStats, IAreIdentical<IAdjective>
    {
        /// <summary>
        /// The object to which the adjective is attached
        /// </summary>
        IHasStats Owner { get; set; }

        /// <summary>
        /// True if the world should form part of the name of the object (e.g. "Dark Room")
        /// </summary>
        bool IsPrefix { get; set; }


        /// <summary>
        /// Describes the effects (positive and negative) of the <see cref="IAdjective"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetDescription();


    }
}

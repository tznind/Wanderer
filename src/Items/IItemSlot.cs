using System.Collections.Generic;
using Wanderer.Adjectives;

namespace Wanderer.Items
{
    /// <summary>
    /// A location in which an item can be equipped
    /// </summary>
    public interface IItemSlot
    {
        /// <summary>
        /// The name of the location e.g. Head
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The number of spare slots that the user must have e.g. a
        /// big hammer might require 2 hands to use
        /// </summary>
        int NumberRequired { get; set; }

        /// <summary>
        /// If you sustain injuries to any of these locations then
        /// your item might not work so well.  E.g. Arm injury may inhibit
        /// your ability to shoot a gun
        /// </summary>
        InjuryRegion[] SensitiveTo { get; set; }
    }
}
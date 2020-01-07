using System;

namespace StarshipWanderer.Stats
{
    public static class StatExtensions
    {
        public static string Describe(this Stat s)
        {
            switch (s)
            {
                case Stat.None:
                    return "Not used";
                case Stat.Loyalty:
                    return "Your outward appearance of loyalty to the Emperor";
                case Stat.Corruption:
                    return "How much of your soul is missing";
                case Stat.Fight:
                    return "How good you are at hurting others";
                case Stat.Coerce:
                    return "How convincing you are to others";
                case Stat.Savvy:
                    return "How good you are with tech";
                case Stat.Initiative:
                    return "Determines action order for Npcs";
                default:
                    throw new ArgumentOutOfRangeException(nameof(s), s, null);
            }
        }
    }
}
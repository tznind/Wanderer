using System;

namespace Wanderer.Stats
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
                case Stat.Value:
                    return "How much something or someone is worth";
                case Stat.Leadership:
                    return "How good you are at commanding allies";
                case Stat.Suave:
                    return "How tolerant others are of your bad behaviours";
                default:
                    throw new ArgumentOutOfRangeException(nameof(s), s, null);
            }
        }
    }
}
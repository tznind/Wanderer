using System;
using System.Linq;
using StarshipWanderer.Adjectives;

namespace StarshipWanderer.Actors
{
    public abstract class HasStatsFactory<T> where T : IHasStats
    {
        public IAdjectiveFactory AdjectiveFactory { get; set; }

        
        public void Add<T2>(T o) where T2 : IAdjective
        {
            var match = AdjectiveFactory.GetAvailableAdjectives(o).OfType<T2>().FirstOrDefault();

            if (match == null)
                throw new ArgumentException($"AdjectiveFactory did not know how to make an item {typeof(T)}.  Try adding it to AdjectiveFactory.GetAvailableAdjectives(IItem)");

            o.Adjectives.Add(match);
        }
    }
}
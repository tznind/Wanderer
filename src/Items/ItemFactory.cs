using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Items
{
    public class ItemFactory : IItemFactory
    {
        public AdjectiveFactory AdjectiveFactory { get; }

        public ItemFactory(AdjectiveFactory adjectiveFactory)
        {
            AdjectiveFactory = adjectiveFactory;
        }

        public IItem Create(IPlace inPlace)
        {
            IItem i;
            inPlace.Items.Add(i = GetRandomItem(inPlace.World.R));

            return i;
        }

        public IItem Create(IActor forActor)
        {
            return GetRandomItem(forActor.CurrentLocation.World.R);
        }
        protected virtual IItem GetRandomItem(Random r)
        {
            var available = GetAvailableItems().ToArray();

            var choose = available[r.Next(available.Length)];
            if (choose is IItemStack s)
                s.StackSize = r.Next(10);

            return choose;
        }

        private IEnumerable<IItem> GetAvailableItems()
        {
            yield return Create<Light>("Globe");
            yield return Create<Tough>("Environment Suit");
            yield return Create<SingleUse,Medic>("Kit");
            yield return new ItemStack("Creds",1).With(Stat.Value,1);
        }

        
        public IItemStack CreateStack<T>(string name, int size) where T : IAdjective
        {
            var item = new ItemStack(name,size);
            Add<T>(item);
            return item;
        }

        public IItem Create<T>(string name) where T : IAdjective
        {
            var item = new Item(name);
            
            Add<T>(item);

            return item;
        }

        public IItem Create<T1,T2>(string name) where T1 : IAdjective where T2:IAdjective
        {
            var item = new Item(name);
            
            Add<T1>(item);
            Add<T2>(item);

            return item;
        }

        private void Add<T>(Item item) where T : IAdjective
        {
            var match = AdjectiveFactory.GetAvailableAdjectives(item).OfType<T>().FirstOrDefault();

            if (match == null)
                throw new ArgumentException($"AdjectiveFactory did not know how to make an item {typeof(T)}.  Try adding it to AdjectiveFactory.GetAvailableAdjectives(IItem)");

            item.Adjectives.Add(match);
        }

    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Places;

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
            
            return GetRandomItem(inPlace.World.R);
        }

        public IItem Create(IActor forActor)
        {
            return GetRandomItem(forActor.CurrentLocation.World.R);
        }
        protected virtual IItem GetRandomItem(Random r)
        {
            var available = GetAvailableItems().ToArray();

            return available[r.Next(available.Length)];
        }

        private IEnumerable<IItem> GetAvailableItems()
        {
            yield return Create<Light>("Globe");
            yield return Create<Tough>("Environment Suit");
            yield return Create<SingleUse,Medic>("Kit");
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
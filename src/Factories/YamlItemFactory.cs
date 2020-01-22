using System;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Extensions;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Factories
{
    public class YamlItemFactory : IItemFactory
    {
        public ItemBlueprint[] Blueprints { get; set; }

        public IAdjectiveFactory AdjectiveFactory { get; set; }

        public YamlItemFactory(string yaml, IAdjectiveFactory adjectiveFactory)
        {
            AdjectiveFactory = adjectiveFactory;

            var deserializer = new Deserializer();
            Blueprints = deserializer.Deserialize<ItemBlueprint[]>(yaml);
        }
        public IItem Create(IPlace inPlace)
        {
            IItem item = Create(Blueprints.GetRandom(inPlace.World.R));
            inPlace.Items.Add(item);
            return item;
        }

        public IItem Create(IActor forActor)
        {
            var item = Create(Blueprints.GetRandom(forActor.CurrentLocation.World.R));
            forActor.Items.Add(item);
            return item;
        }

        public IItem Create(ItemBlueprint blueprint)
        {
            var item = new Item(blueprint.Name);

            if (blueprint.Dialogue != null)
            {
                item.BaseActions.Add(new ReadAction());
                item.NextDialogue = blueprint.Dialogue;
            }

            return item;
        }

    }

 
}
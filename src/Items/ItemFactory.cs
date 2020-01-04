using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Places;

namespace StarshipWanderer.Items
{
    public class ItemFactory : IItemFactory
    {
        public IItem Create(IPlace inPlace)
        {
            return GloGlobe();
        }
        public IItem Create(IActor forActor)
        {
            return GloGlobe();
        }

        private IItem GloGlobe()
        {
            var item = new Item("Glo-Globe");
            item.Adjectives.Add(new Light(item));

            return item;
        }

    }
}
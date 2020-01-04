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
            return GloGlobe();
        }
        public IItem Create(IActor forActor)
        {
            var i = GloGlobe();

            i.OwnerIfAny = forActor;

            return i;
        }

        private IItem GloGlobe()
        {
            var item = new Item("Globe");
            item.Adjectives.Add(new Light(item));

            return item;
        }

    }
}
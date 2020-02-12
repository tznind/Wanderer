using Wanderer.Actions;
using Wanderer.Adjectives;
using Wanderer.Dialogues;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;

namespace Wanderer.Factories
{
    public class ItemFactory : HasStatsFactory<IItem>,IItemFactory
    {
        public ItemBlueprint[] Blueprints { get; set; } = new ItemBlueprint[0];

        public ItemFactory(IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
        }

        public IItem Create(IWorld world,ItemBlueprint blueprint)
        {
            var item = 
                blueprint.Stack.HasValue ?
                new ItemStack(blueprint.Name,blueprint.Stack.Value):
                new Item(blueprint.Name);

            AddBasicProperties(item,blueprint,world,"read");

            if(blueprint.Require != null)
                item.Require = blueprint.Require;

            if (blueprint.Slot != null)
                item.Slot = blueprint.Slot;

            return item;
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

    }
}
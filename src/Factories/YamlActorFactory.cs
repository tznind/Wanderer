using NLog.LayoutRenderers;
using StarshipWanderer.Actors;
using StarshipWanderer.Extensions;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Factories
{
    public class YamlActorFactory : ActorFactory
    {
        public ActorBlueprint[] Blueprints { get; set; }

        public YamlActorFactory(string yaml,IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory) : base(itemFactory, adjectiveFactory)
        {
            var deserializer = new Deserializer();
            Blueprints = deserializer.Deserialize<ActorBlueprint[]>(yaml);
        }

        public override void Create(IWorld world, IPlace place)
        {
            Create(world, place,Blueprints.GetRandom(world.R));
        }

        public IActor Create(IWorld world, IPlace place, ActorBlueprint blueprint)
        {
            var npc = new Npc(blueprint.Name, place);
            
            //pick 1 random adjective
            var adjective = AdjectiveFactory.Create(npc,blueprint.Adjectives.GetRandom(world.R));
            npc.Adjectives.Add(adjective);

            return npc;
        }
    }
}
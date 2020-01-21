using Newtonsoft.Json;
using StarshipWanderer.Actors;
using StarshipWanderer.Extensions;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Factories
{
    public class YamlActorFactory : ActorFactory
    {
        public ActorBlueprint[] Blueprints { get; set; }

        [JsonConstructor]
        private YamlActorFactory(IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory):base(itemFactory,adjectiveFactory)
        {

        }

        public YamlActorFactory(string yaml,IFaction faction,IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory) : base(itemFactory, adjectiveFactory)
        {
            FactionIfAny = faction;
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

            if (FactionIfAny != null)
                npc.FactionMembership.Add(FactionIfAny);

            if(string.IsNullOrWhiteSpace(npc.Name))
                npc.Name = FactionIfAny?.NameFactory?.GenerateName(world.R) ?? "Unnamed Npc";

            return npc;
        }
    }
}
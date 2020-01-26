using System;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Extensions;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;

namespace StarshipWanderer.Factories
{
    public class ActorFactory : HasStatsFactory<IActor> ,IActorFactory
    {
        public ActorBlueprint[] Blueprints { get; set; }
        
        public IItemFactory ItemFactory { get; set; }

        public ActorFactory(IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            ItemFactory = itemFactory;
        }
        
        public virtual void Create(IWorld world, IPlace place,IFaction faction)
        {
            int numberOfNpc = Math.Max(1,world.R.Next(5));

            if(Blueprints.Any())
                for (int i = 0; i < numberOfNpc; i++)
                    Create(world, place, faction, Blueprints.GetRandom(world.R));
        }

        public IActor Create(IWorld world, IPlace place,IFaction faction, ActorBlueprint blueprint)
        {
            var npc = new Npc(blueprint.Name, place);
            
            //Adjectives the user definitely wants included
            if (blueprint.MandatoryAdjectives.Any())
                foreach (var a in blueprint.MandatoryAdjectives)
                    npc.Adjectives.Add(AdjectiveFactory.Create(npc, a));
            
            //pick 1 random adjective if blueprint lists any to pick from
            if (blueprint.OptionalAdjectives.Any())
                npc.Adjectives.Add(
                    AdjectiveFactory.Create(npc, blueprint.OptionalAdjectives.GetRandom(world.R))
                    );

            if (blueprint.Stats != null)
                npc.BaseStats.Add(blueprint.Stats);

            if (faction != null)
                npc.FactionMembership.Add(faction);

            if (blueprint.Dialogue != null)
            {
                npc.Dialogue = blueprint.Dialogue;
                if (npc.Dialogue.Verb == null)
                    npc.Dialogue.Verb = "talk";
            }

            if(string.IsNullOrWhiteSpace(npc.Name))
                npc.Name = faction?.NameFactory?.GenerateName(world.R) ?? "Unnamed Npc";

            foreach (var blue in blueprint.Items)
            {
                var item = ItemFactory.Create(blue);
                npc.Items.Add(item);

                if (npc.CanEquip(item, out _))
                    item.IsEquipped = true;


            }

            return npc;
        }
    }
}

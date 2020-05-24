using System;
using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class SpawnEffect : Effect
    {
        public string ToSpawn { get; }

        public SpawnEffect(string toSpawn, SystemArgsTarget target) : base(target)
        {
            ToSpawn = toSpawn;
        }

        
        public override void Apply(SystemArgs args)
        {
            var onto = args.GetTarget(Target);
            var blue = args.World.GetBlueprint(ToSpawn);

            if(blue == null)
                throw new NamedObjectNotFoundException($"Could not find Blueprint {ToSpawn}",ToSpawn);

            if(blue is RoomBlueprint)   
                throw new NotSupportedException("Spawn Effect cannot be used to spawn a Room");
            
            if(blue is ActorBlueprint actorBlueprint)
                args.World.ActorFactory.Create(args.World,onto is IRoom r ? r : args.Room,null,actorBlueprint,null);

            if(blue is ItemBlueprint itemBlueprint)
            {
                if(onto is IActor a)
                    a.SpawnItem(itemBlueprint);
                if(onto is IRoom r)
                    r.SpawnItem(itemBlueprint);
            }

            if(blue is AdjectiveBlueprint adjectiveBlueprint)
                args.World.AdjectiveFactory.Create(args.World,onto,adjectiveBlueprint);
            
            if(blue is ActionBlueprint actionBlueprint)
                args.World.ActionFactory.Create(args.World,onto,actionBlueprint);

            if(blue is BehaviourBlueprint behaviourBlueprint)
                args.World.BehaviourFactory.Create(args.World,onto,behaviourBlueprint);
        }
    }
}
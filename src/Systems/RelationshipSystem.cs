using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Relationships;

namespace StarshipWanderer.Systems
{
    public class RelationshipSystem : List<IRelationship>, IRelationshipSystem
    {
        public void Apply(SystemArgs args)
        {
            //don't form a relationship with yourself or nobody!
            if(args.AggressorIfAny == args.Recipient || args.AggressorIfAny == null)
                return;
            
            var actorRecipient = (IActor)args.Recipient;

            //if someone is doing something to you
            if (Math.Abs(args.Intensity) > 0.0001)
            {
                //don't form relationships with the dead
                if(args.AggressorIfAny.Dead || actorRecipient.Dead)
                    return;

                var world = actorRecipient.CurrentLocation.World;

                //log the change
                if(args.Intensity > 0.001)
                    args.UserInterface.Log.Info(new LogEntry($"{args.Recipient} expressed approval towards {args.AggressorIfAny}",args.Round,actorRecipient));
                else if (args.Intensity < 0.001)
                    args.UserInterface.Log.Info(new LogEntry($"{args.Recipient} became angry at {args.AggressorIfAny}",args.Round,actorRecipient));

                //then you need to be angry about that! (or happy)
                var existingRelationship = world.Relationships.OfType<PersonalRelationship>()
                    .SingleOrDefault(r => r.AppliesTo(actorRecipient, args.AggressorIfAny));

                if (existingRelationship != null)
                    existingRelationship.Attitude += args.Intensity;
                else
                    world.Relationships.Add(new PersonalRelationship(actorRecipient, args.AggressorIfAny)
                        {Attitude = args.Intensity});
            }
        }

        public double SumBetween(IActor observer, IActor observed)
        {
            return this.Where(o => o.AppliesTo(observer, observed)).Sum(a => a.Attitude);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Relationships;

namespace Wanderer.Systems
{
    public class RelationshipSystem : List<IRelationship>, IRelationshipSystem
    {

        
        /// <summary>
        /// How much an attitude change effects inter faction relationships.  e.g.
        /// If you attack someone they hate you for 15 but any factions also hate your
        /// faction(s) for 1.5.  This value is a multiplier and should normally be between
        ///  0 and 1
        /// </summary>
        double factionFraction = 0.1;

        public Guid Identifier { get; set; } = new Guid("8cfad1e6-39f9-4993-831f-57234290f558");

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


                ApplyChangeToPersonalRelationship(world,actorRecipient,args);
                ApplyChangeToFactionRelationships(world,actorRecipient,args);
                
            }
        }

        private void ApplyChangeToPersonalRelationship(IWorld world,IActor actorRecipient,SystemArgs args)
        {

                //then you need to be angry about that! (or happy)
                var existingRelationship = world.Relationships.OfType<PersonalRelationship>()
                    .FirstOrDefault(r => r.AppliesTo(actorRecipient, args.AggressorIfAny));

                if (existingRelationship != null)
                    existingRelationship.Attitude += args.Intensity;
                else
                    world.Relationships.Add(new PersonalRelationship(actorRecipient, args.AggressorIfAny)
                        {Attitude = args.Intensity});
        }


        private void ApplyChangeToFactionRelationships(IWorld world,IActor actorRecipient,SystemArgs args)
        {

                //each faction you represent as the victim/recipient
                foreach(var receiving in actorRecipient.FactionMembership)
                {
                    foreach(var aggressing in args.AggressorIfAny.FactionMembership)
                    {
                        var existingRelationship = world.Relationships.OfType<InterFactionRelationship>()
                            .FirstOrDefault(r => r.HostFaction == receiving && r.ObservedFaction == aggressing);


                        if (existingRelationship != null)
                            existingRelationship.Attitude += args.Intensity * factionFraction;
                        else
                            world.Relationships.Add(new InterFactionRelationship(receiving,aggressing, args.Intensity * factionFraction)
                                {Attitude = args.Intensity});
                    }
                }


        }

        public double SumBetween(IActor observer, IActor observed)
        {
            return this.Where(o => o.AppliesTo(observer, observed)).Sum(a => a.Attitude);
        }
    }
}
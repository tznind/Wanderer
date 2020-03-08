using System;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Rooms;

namespace Wanderer.Systems
{
    public class Spreading
    {
        public bool RoomsToRooms { get; set; }
        public bool RoomsToActors { get; set; }
        public bool ActorsToActors { get; set; }
        public bool ActorsToRooms { get; set; }

        internal void HandleSpreading(Injured injured, InjurySystem injurySystem,IUserinterface ui,Guid round)
        {
                   
            if(injured.Owner is IRoom p)
            {
                if(RoomsToRooms)
                    foreach(var adjacent in p.World.Map.GetAdjacentRooms(p,false))
                        injurySystem.Apply(new SystemArgs(p.World,ui,1,null,p,round));

                if(RoomsToActors)
                    foreach(var actor in p.Actors)
                        injurySystem.Apply(new SystemArgs(p.World,ui,1,null,actor,round));
            }
            
            if(injured.Owner is IActor a)
            {
                var world = a.CurrentLocation.World;
                
                if(ActorsToActors)
                    foreach(var actor in a.GetCurrentLocationSiblings(false))
                        injurySystem.Apply(new SystemArgs(world,ui,1,null,actor,round));

                if(ActorsToRooms)
                    injurySystem.Apply(new SystemArgs(world,ui,1,null,a.CurrentLocation,round));
            }
        }
    }
}
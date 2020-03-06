using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Wanderer.Plans
{
    public class FollowPlan : Plan, IFrameSource, ICondition<SystemArgs>
    {
        public IActor ToFollow { get; set; }

        [JsonConstructor]
        protected FollowPlan()
        {
            
        }

        public FollowPlan(IActor toFollow)
        {
            ToFollow = toFollow;
            Do = this;
            Condition = new List<ICondition<SystemArgs>> {this};
        }


        public Frame GetFrame(SystemArgs args)
        {
            var path = GetPath(args);

            if (path == null || path.Count < 2)
                return null;

            Direction toMove = Direction.None;

            foreach (var adjacentRoom in args.World.Map.GetAdjacentRooms(path[0].Room,true))
                if (adjacentRoom.Value == path[1].Room)
                    toMove = adjacentRoom.Key;

            //Somehow we are no longer able to go that way :(
            if (toMove == Direction.None)
                return null;

            return new LeaveFrame((IActor) args.Recipient, new LeaveAction(),toMove,0);
        }


        public bool IsMet(IWorld world,SystemArgs args)
        {
            if (!(args.Recipient is IActor a))
                return false;
            
            return 
                //must be able to take wander around
                /*args.GetFinalAction<LeaveAction>() != null &&*/
                
                //and both are alive (both the follower and the person they are following
                !ToFollow.Dead && !a.Dead &&
                
                //and they are not already right next to you
                a.DistanceTo(ToFollow) >= 1 &&
                
                //and it is possible to navigate a path to them
                GetPath(args) != null;
        }

        public List<DijkstraPathing.Node> GetPath(SystemArgs args)
        {
            var from = args.Room;
            var to = ToFollow.CurrentLocation;

            DijkstraPathing pathing = new DijkstraPathing(args.World.Map,from,to);
            return pathing.GetShortestPathDijkstra();
        }

        public bool IsMet(IWorld world,object o)
        {
            return IsMet(world,(SystemArgs) o);
        }
    }
}

using StarshipWanderer.Actors;

namespace StarshipWanderer.Actions
{
    public class LeaveFrame : Frame
    {
        public Direction Direction { get; set; }

        public LeaveFrame(IActor actor,IAction action, Direction direction):base(actor,action)
        {
            Direction = direction;
        }
    }
}
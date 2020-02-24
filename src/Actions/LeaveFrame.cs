using Wanderer.Actors;

namespace Wanderer.Actions
{
    public class LeaveFrame : Frame
    {
        public Direction LeaveDirection { get; set; }

        public LeaveFrame(IActor actor,IAction action, Direction direction,double attitude):base(actor,action,attitude)
        {
            LeaveDirection = direction;
        }
    }
}
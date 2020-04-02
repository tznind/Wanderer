using Wanderer.Systems;

namespace Wanderer.Behaviours
{
    public class ActionFrameSystemArgs : SystemArgs
    {

        public ActionStack Stack {get;set;}
        public Frame Frame {get;set;}

        public ActionFrameSystemArgs(IWorld world, IUserinterface ui,ActionStack stack, Frame frame ): base(world,ui,0,frame.PerformedBy,frame.TargetIfAny ?? frame.PerformedBy,stack.Round)
        {
            Stack = stack;
            Frame = frame;
        }
    }
}
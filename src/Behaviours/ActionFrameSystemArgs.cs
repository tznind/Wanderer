namespace Wanderer.Behaviours
{
    public class ActionFrameSystemArgs : EventSystemArgs
    {
        public ActionStack Stack {get;set;}
        public Frame Frame {get;set;}

        public ActionFrameSystemArgs(IHasStats source,IWorld world, IUserinterface ui,ActionStack stack, Frame frame ): 
            base(source,world,ui,frame.PerformedBy,frame.TargetIfAny ?? frame.PerformedBy,stack.Round)
        {
            Stack = stack;
            Frame = frame;

            if (Action == null)
                Action = frame.Action;
        }
    }
}
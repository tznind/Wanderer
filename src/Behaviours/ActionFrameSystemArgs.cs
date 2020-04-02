namespace Wanderer.Behaviours
{
    public class ActionFrameSystemArgs : BehaviourSystemArgs
    {
        public ActionStack Stack {get;set;}
        public Frame Frame {get;set;}

        public ActionFrameSystemArgs(IBehaviour behaviour,IWorld world, IUserinterface ui,ActionStack stack, Frame frame ): 
            base(behaviour,world,ui,frame.PerformedBy,frame.TargetIfAny ?? frame.PerformedBy,stack.Round)
        {
            Stack = stack;
            Frame = frame;
        }
    }
}
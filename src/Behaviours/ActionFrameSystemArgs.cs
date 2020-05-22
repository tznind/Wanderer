namespace Wanderer.Behaviours
{
    /// <summary>
    /// SystemArgs describing an action being performed on the <see cref="ActionStack"/>
    /// </summary>
    public class ActionFrameSystemArgs : EventSystemArgs
    {
        /// <summary>
        /// The current stack of actions that have been pushed
        /// </summary>
        public ActionStack Stack {get;set;}

        /// <summary>
        /// The packaged command that is currently being considered, including targets
        /// </summary>
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
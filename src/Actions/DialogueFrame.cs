using StarshipWanderer.Actors;

namespace StarshipWanderer.Actions
{
    public class DialogueFrame : Frame
    {
        public IHasStats DialogueTarget { get; }

        public DialogueFrame(IActor actor, Action action, IHasStats dialogueTarget, int attitude):base(actor,action,attitude)
        {
            DialogueTarget = dialogueTarget;
        }
    }
}
using StarshipWanderer.Systems;

namespace StarshipWanderer.Dialogues.Conditions
{
    public interface IDialogueCondition
    {
        bool IsMet(SystemArgs dialogueArgs);
    }
}
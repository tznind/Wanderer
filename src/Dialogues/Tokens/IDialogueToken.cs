using StarshipWanderer.Systems;

namespace StarshipWanderer.Dialogues.Tokens
{
    public interface IDialogueToken
    {
        string[] Tokens { get; }
        string GetReplacement(SystemArgs dialogueArgs);
    }
}
using StarshipWanderer.Systems;

namespace StarshipWanderer.Dialogues.Conditions
{
    public interface IDialogueCondition
    {
        bool IsMet(SystemArgs dialogueArgs);

        /// <summary>
        /// Return the constructor call that would be used to create new instances
        /// of this class e.g. MyClass(5,3).  This is the yaml the user would type
        /// to create this parameterized condition (and so may allow you to drop
        /// off suffixes e.g. CheckSomething(1) for an ICondition called CheckSomethingCondition
        /// </summary>
        /// <returns></returns>
        string? SerializeAsConstructorCall();
    }
}
using System.Collections.Generic;
using System.Linq;

namespace StarshipWanderer.Dialogues.Conditions
{
    public class DialogueConditionCollection : List<IDialogueCondition>
    {
        public DialogueConditionCollection()
        {
            
        }
        public DialogueConditionCollection(params IDialogueCondition[] conditions)
        {
            if(conditions.Any())
                AddRange(conditions);
        }
    }
}
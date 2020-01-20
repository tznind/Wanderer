using StarshipWanderer.Actors;
using StarshipWanderer.Items;

namespace StarshipWanderer.Actions
{
    public class EquipmentFrame : Frame
    {
        public EquipmentActionToPerform ToPerform { get; }
        public IItem Item { get; }

        public EquipmentFrame(IActor actor, EquipmentAction action, EquipmentActionToPerform toPerform, IItem chosen):base(actor,action,0)
        {
            ToPerform = toPerform;
            Item = chosen;
        }
    }
}
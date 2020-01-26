using System;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;

namespace StarshipWanderer.Actions
{
    public class EquipmentAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            if (actor.Decide(ui, "Equipment Action", null, out EquipmentActionToPerform toPerform,
                GetValues<EquipmentActionToPerform>(),0))
            {
                switch (toPerform)
                {
                    case EquipmentActionToPerform.None:
                        break;
                    case EquipmentActionToPerform.PutOn:
                        if(actor.Decide(ui, "Item", "Pick an item to put on", out IItem equip,actor.Items.ToArray(),0))
                            if(actor.CanEquip(equip,out string s))
                                stack.Push(new EquipmentFrame(actor,this,toPerform,equip));
                            else
                                ui.ShowMessage("Cannot Equip",s);
                            
                        break;
                    case EquipmentActionToPerform.TakeOff:
                        if(actor.Decide(ui, "Item", "Pick an item to take off", out IItem remove,actor.Items.Where(i=>i.IsEquipped).ToArray(),0))
                            stack.Push(new EquipmentFrame(actor,this,toPerform,remove));

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (EquipmentFrame)frame;

            switch (f.ToPerform)
            {
                case EquipmentActionToPerform.None:
                    break;
                case EquipmentActionToPerform.PutOn:
                    if (f.PerformedBy.CanEquip(f.Item, out string reason))
                        f.Item.IsEquipped = true;
                    else
                    {
                        ShowNarrative(ui,f.PerformedBy,"Equip failed",$"You struggle to equip {f.Item} but find it just won't fit ({reason})",null,stack.Round);
                    }

                    break;
                case EquipmentActionToPerform.TakeOff:
                    f.Item.IsEquipped = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum EquipmentActionToPerform
    {
        None,
        PutOn,
        TakeOff
    }
}
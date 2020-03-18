using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Items;

namespace Wanderer.Actions
{
    public class EquipmentAction : Action
    {
        private EquipmentAction():base(null)
        {
            
        }

        public EquipmentAction(IHasStats owner):base(owner)
        {
            
        }
        public override char HotKey => 'q';
        
        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            if (actor.Decide(ui, "Equipment Action", null, out EquipmentActionToPerform toPerform,
                GetValues<EquipmentActionToPerform>(),0))
            {
                switch (toPerform)
                {
                    case EquipmentActionToPerform.None:
                        break;
                    case EquipmentActionToPerform.PutOn:
                        if(actor.Decide(ui, "Item", "Pick an item to put on", out IItem equip,actor.Items.Where(i=> i.Slot != null && !i.IsEquipped).ToArray(),0))
                            if(actor.CanEquip(equip,out string s))
                                stack.Push(new EquipmentFrame(actor,this,toPerform,equip));
                            else
                                ShowNarrative(ui,actor,"Cannot Equip",s,$"{actor} tried to equip {equip} but failed ({s})",stack.Round);
                            
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

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
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

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }

        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            return performer.Items.Where(i => i.IsEquipped || i.Slot != null);
        }
    }

    public enum EquipmentActionToPerform
    {
        None,
        PutOn,
        TakeOff
    }
}
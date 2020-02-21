using System;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Items;

namespace Wanderer.Behaviours
{
    public class MergeStacksBehaviour : Behaviour
    {
        public MergeStacksBehaviour(IActor owner):base(owner)
        {
        }

        public override void OnRoundEnding(IWorld world,IUserinterface ui, Guid round)
        {
            var a = (IActor)Owner;

            foreach (var s1 in a.Items.OfType<IItemStack>().ToArray())
                foreach (var s2 in a.Items.OfType<IItemStack>().ToArray())
                {
                    if (s1.CanCombine(s2))
                        s1.Combine(s2,world);
                }

            base.OnRoundEnding(world, ui, round);
        }
    }
}
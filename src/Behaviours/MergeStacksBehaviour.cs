using System;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Items;

namespace StarshipWanderer.Behaviours
{
    public class MergeStacksBehaviour : Behaviour
    {
        public MergeStacksBehaviour(IActor owner):base(owner)
        {
        }

        public override void OnRoundEnding(IUserinterface ui, Guid round)
        {
            var a = (IActor)Owner;
            var world = a.CurrentLocation.World;

            foreach (var s1 in a.Items.OfType<IItemStack>().ToArray())
            foreach (var s2 in a.Items.OfType<IItemStack>().ToArray())
            {
                if (s1.CanCombine(s2))
                    s1.Combine(s2,world);
            }

            base.OnRoundEnding(ui, round);
        }
    }
}
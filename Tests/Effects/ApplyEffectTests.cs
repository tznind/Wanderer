using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Tests.Effects
{
    public class ApplyEffectTests : UnitTest
        { 
            [TestCase(SystemArgsTarget.Aggressor,false)]
            [TestCase(SystemArgsTarget.Recipient,false)]
            [TestCase(SystemArgsTarget.Aggressor,true)]
            public void Test_ApplySystem(SystemArgsTarget target, bool toAll)
            {
                TwoInARoom(out You you, out IActor them, out IWorld world);

                world.InjurySystems.Add(new InjurySystem(){
                    Name = "Violence",
                    Injuries = new List<InjuryBlueprint>{new InjuryBlueprint(){Name = "Cracked Rib"}}
                });

                var blue = new EffectBlueprint()
                {
                    Target = target,
                    Apply = new ApplySystemBlueprint()
                    {
                        Name = "Violence",
                        Intensity = 40,
                        All = toAll
                    }
                };

                var effect = (ApplyEffect)blue.Create().Single();

                effect.Apply(new SystemArgs(world,new FixedChoiceUI(),0,you,them,Guid.NewGuid()));

                if(toAll)
                {
                    Assert.IsTrue(you.Has("Injured"));
                    Assert.IsTrue(them.Has("Injured"));
                }
                else
                if(target == SystemArgsTarget.Aggressor)
                {
                    Assert.IsTrue(you.Has("Injured"));
                    Assert.IsFalse(them.Has("Injured"));
                }
                else
                if(target == SystemArgsTarget.Recipient)
                {
                    Assert.IsFalse(you.Has("Injured"));
                    Assert.IsTrue(them.Has("Injured"));
                }

                
            }
        }
}

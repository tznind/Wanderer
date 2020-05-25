using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Factories;
using Wanderer.Systems;
using Wanderer.Systems.Validation;

namespace Tests.Cookbook
{
    abstract class Recipe : UnitTest
    {
        protected IWorld SetupRoom(string roomYaml)
        {
            return Setup("rooms.yaml", roomYaml);
        }

        protected IWorld SetupRoom(string roomYaml, string itemsYaml)
        {
            return Setup("rooms.yaml", roomYaml,"items.yaml",itemsYaml);
        }
        
        protected IWorld SetupDialogue(string dialogueYaml)
        {
            return Setup("dialogue.yaml", dialogueYaml);
        }
        
        protected IWorld SetupItem(string slotsYaml, string itemYaml)
        {
            return Setup("slots.yaml", slotsYaml,"items.yaml",itemYaml);
        }
        
        protected IWorld SetupItem(string itemYaml)
        {
            return Setup("items.yaml",itemYaml);
        }
        protected IWorld SetupItem(string slotsYaml, string itemYaml,string adjectivesYaml)
        {
            return Setup("slots.yaml", slotsYaml,"items.yaml",itemYaml,"adjectives.yaml",adjectivesYaml);
        }

        protected IWorld SetupItem(string slotsYaml, string itemYaml,string adjectivesYaml, string injurySystemYaml)
        {
            return Setup("slots.yaml", slotsYaml,"items.yaml",itemYaml,"adjectives.yaml",adjectivesYaml,"InjurySystems/system1.injury.yaml",injurySystemYaml);
        }
        protected IWorld Setup(params string[] pairs)
        {
            List<WorldFactoryResource> resources = new List<WorldFactoryResource>();

            for(int i=0;i<pairs.Length;i+=2) 
                resources.Add(new WorldFactoryResource(pairs[i], pairs[i + 1] ));

            var wf = new CookBookWorldFactory(resources);

            //make sure that the directory setup passes validation
            var validator = new WorldValidator();
            validator.Validate(wf);

            Assert.AreEqual(0,validator.Warnings.Length,"Recipe resulted in warnings during world validation: " + validator.Warnings);
            Assert.AreEqual(0,validator.Errors.Length,"Recipe resulted in errors during world validation: " + validator.Errors);

            return wf.Create();
        }

        protected void GoWest(IWorld world)
        {
            GoWest(world, out _);
        }
        protected void GoWest(IWorld world, out FixedChoiceUI ui)
        {
            world.RunRound(ui = GetUI(Direction.West),new LeaveAction(world.Player));
        }
        protected void GoEast(IWorld world)
        {
            GoEast(world, out _);
        }
        
        protected void GoEast(IWorld world, out FixedChoiceUI ui)
        {
            world.RunRound(ui = GetUI(Direction.East),new LeaveAction(world.Player));
        }
        protected string RunDialogue(IWorld world)
        {
            var ui = GetUI();
            var npc = new Npc("test actor", world.Player.CurrentLocation);
            world.Dialogue.Run(
                new SystemArgs(world,ui,0,world.Player,npc,Guid.Empty),
                world.Dialogue.AllDialogues.First());

            return string.Join(Environment.NewLine,ui.MessagesShown);
        }


    }
}

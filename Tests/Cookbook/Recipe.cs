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

namespace Tests.Cookbook
{
    abstract class Recipe : UnitTest
    {
        protected IWorld SetupRoom(string roomYaml)
        {
            return Setup("Rooms.yaml", roomYaml);
        }
        
        protected IWorld SetupDialogue(string dialogueYaml)
        {
            return Setup("Dialogue.yaml", dialogueYaml);
        }
        
        protected IWorld SetupItem(string slotsYaml, string itemYaml)
        {
            return Setup("Slots.yaml", slotsYaml,"Items.yaml",itemYaml);
        }
        private IWorld Setup(params string[] pairs)
        {
            var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, Path.GetRandomFileName());
            Directory.CreateDirectory(dir);

            for(int i=0;i<pairs.Length;i+=2) 
                File.WriteAllText(Path.Combine(dir, pairs[i]), pairs[i + 1]);


            var wf = new WorldFactory() {ResourcesDirectory = dir};

            return wf.Create();
        }

        protected void GoWest(IWorld world)
        {
            world.RunRound(GetUI(Direction.West),new LeaveAction(world.Player));
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

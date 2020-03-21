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

        private IWorld Setup(string filename, string yaml)
        {
            var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, Path.GetRandomFileName());
            Directory.CreateDirectory(dir);

            File.WriteAllText(Path.Combine(dir,filename),yaml);

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

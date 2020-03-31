﻿using System;
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

        protected IWorld SetupItem(string slotsYaml, string itemYaml,string adjectivesYaml)
        {
            return Setup("slots.yaml", slotsYaml,"items.yaml",itemYaml,"adjectives.yaml",adjectivesYaml);
        }

        protected IWorld SetupItem(string slotsYaml, string itemYaml,string adjectivesYaml, string injurySystemYaml)
        {
            return Setup("slots.yaml", slotsYaml,"items.yaml",itemYaml,"adjectives.yaml",adjectivesYaml,"InjurySystems/system1.injury.yaml",injurySystemYaml);
        }
        private IWorld Setup(params string[] pairs)
        {
            var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, Path.GetRandomFileName());
            Directory.CreateDirectory(dir);

            for(int i=0;i<pairs.Length;i+=2) 
            {

                var fi = new FileInfo(Path.Combine(dir, pairs[i]));

                if(!fi.Directory.Exists)
                    fi.Directory.Create();

                File.WriteAllText(fi.FullName, pairs[i + 1]);
            }

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

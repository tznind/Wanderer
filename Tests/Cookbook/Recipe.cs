using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Factories;

namespace Tests.Cookbook
{
    abstract class Recipe : UnitTest
    {
        protected IWorld Setup(string roomYaml)
        {
            var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, Path.GetRandomFileName());
            Directory.CreateDirectory(dir);

            File.WriteAllText(Path.Combine(dir,"Rooms.yaml"),roomYaml);

            var wf = new WorldFactory() {ResourcesDirectory = dir};

            return wf.Create();
        }
        protected void GoWest(IWorld world)
        {
            world.RunRound(GetUI(Direction.West),new LeaveAction(world.Player));
        }
    }
}

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Factories;
using Wanderer.Rooms;
using Wanderer.Relationships;

namespace Tests
{
    public class UnitTest
    {
        protected IRoom InARoom(out IWorld world)
        {
            var wf = new WorldFactory();
            wf.ResourcesDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources");
            wf.SkipContent = true;
            world = wf.Create();
            world.Population.Clear();
            world.Relationships.Clear();
            world.Factions.Clear();
            world.RoomFactory.Blueprints.Clear();
            world.ItemFactory.Blueprints.Clear();
            world.ActorFactory.Blueprints.Clear();
            world.Dialogue.AllDialogues.Clear();

            var room = new Room("TestRoom", world,'-');
            world.Map.Clear();
            world.Map.Add(new Point3(0,0,0),room );
            
            return room;
        }

        protected You YouInARoom(out IWorld world)
        {
            var room = InARoom(out world);
            var you = new You("Test Wanderer", room);

            WorldFactory.AddDefaults(world,you);
            return you;
        }
        
        protected FixedChoiceUI GetUI(params object[] choiceSelection)
        {
            return new FixedChoiceUI(choiceSelection);
        }
        
        public void RunRound(IWorld world, string actionName,params object[] uiChoices)
        {
            var actions = world.Player.GetFinalActions();

            Assert.AreEqual(1,actions.Count(a=>a.ToString() == actionName),$"Failed to find action {actionName}.  Player actions included: {string.Join(Environment.NewLine,actions)}");
            
            var ui = GetUI(uiChoices);
            world.RunRound(ui,actions.Single(a=>a.ToString() == actionName));

            if(ui.MessagesShown.Count == 0)
                TestContext.Out.WriteLine("Round Run but no messages shown");

            foreach(var msg in ui.MessagesShown)
                TestContext.Out.WriteLine(msg);
        }
        protected void TwoInARoom(out You you, out IActor them, out IWorld w)
        {
            you = YouInARoom(out w);

            them = new Npc("Chaos Sam", you.CurrentLocation);

            //don't wonder off Chaos Sam
            them.BaseActions.Remove(new LeaveAction(you));
        }
        /// <summary>
        /// Creates a relationship of strength <paramref name="intensity"/> which is how strongly
        /// <paramref name="them"/> feels about <paramref name="you"/>
        /// </summary>
        /// <param name="attitude"></param>
        /// <param name="youFeelTheSameWay">If you feel the same back then two relationships will be created instead of one (i.e. in both directions)</param>
        /// <param name="you"></param>
        /// <param name="them"></param>
        /// <param name="w"></param>
        protected void TwoInARoomWithRelationship(double attitude,bool youFeelTheSameWay, out You you, out IActor them, out IWorld w)
        {
            TwoInARoom(out you, out them, out w);
            w.Relationships.Add(new PersonalRelationship(them, you){Attitude = attitude});

            if(youFeelTheSameWay)
                w.Relationships.Add(new PersonalRelationship(you,them ){Attitude = attitude});
        }
    }
}
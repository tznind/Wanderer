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
            world = wf.Create();
            world.Population.Clear();
            world.Relationships.Clear();
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
            return new You("Test Wanderer", room);
        }
        
        protected FixedChoiceUI GetUI(params object[] choiceSelection)
        {
            return new FixedChoiceUI(choiceSelection);
        }

        protected void TwoInARoom(out You you, out IActor them, out IWorld w)
        {
            you = YouInARoom(out w);

            them = new Npc("Chaos Sam", you.CurrentLocation);

            //don't wonder off Chaos Sam
            them.BaseActions.Remove(new LeaveAction());
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
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace Tests
{
    public class UnitTest
    {
        protected IPlace InARoom(out IWorld world)
        { 
            /*
            var world = new World();
            world.Factions = new FactionCollection();

            var adjectiveFactory = new AdjectiveFactory();
            var itemFactory = new ItemFactory(adjectiveFactory);

            var roomFactory = new RoomFactory(new GenericActorFactory(itemFactory, adjectiveFactory),itemFactory, adjectiveFactory);
            */


            world = new World();
            var room = new Room("TestRoom", world,'-');
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
    }
}
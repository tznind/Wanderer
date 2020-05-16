using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Rooms;

namespace Tests.Adjectives
{
    class AdjectiveFactoryTests : UnitTest
    {
        [TestCase("Strong")]
        [TestCase("Tough")]
        [TestCase("Rusty")]
        public void TestCreate_AdjectiveBlueprint(string typeName)
        {
            var you = YouInARoom(out IWorld w);
            Assert.IsNotNull(w.AdjectiveFactory.Create(w,you,typeName));
        }

        [TestCase("Dark")]
        public void TestCreateRoomOnly_AdjectiveBlueprint(string typeName)
        {
            var room = InARoom(out IWorld w);
            Assert.IsNotNull(w.AdjectiveFactory.Create(w,room,"Dark"));
        }
    }
}
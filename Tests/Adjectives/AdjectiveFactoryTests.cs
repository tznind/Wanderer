using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Adjectives.RoomOnly;
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
            InARoom(out IWorld w);
            Assert.IsNotNull(w.AdjectiveFactory.Create(Mock.Of<IHasStats>(),typeName));
        }

        [TestCase("Dark")]
        public void TestCreateRoomOnly_AdjectiveBlueprint(string typeName)
        {
            var adj = new AdjectiveFactory();
            Assert.IsNotNull(adj.Create(Mock.Of<IRoom>(),typeof(Dark)));
        }
    }
}
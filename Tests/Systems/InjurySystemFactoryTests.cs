using NUnit.Framework;

namespace Tests.Systems
{
    public class InjurySystemFactoryTests
    {
        [Test]
        public void TestCreatingInjurySystem()
        {
            new InjurySystemFactory();
        }
    }
}
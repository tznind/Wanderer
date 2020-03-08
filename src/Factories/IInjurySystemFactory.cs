using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Factories
{
    internal interface IInjurySystemFactory
    {
        IInjurySystem Create(InjurySystemBlueprint blue);
    }
}
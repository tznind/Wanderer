using Wanderer.Adjectives;

namespace Wanderer.Factories.Blueprints
{
    public class InjuryBlueprint
    {
        public InjuryRegion Region { get; set; } = InjuryRegion.None;
        public string Name { get; set; }
        public double Severity { get; set; }
    }
}
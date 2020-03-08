using Wanderer.Adjectives;

namespace Wanderer.Factories.Blueprints
{
    public class InjuryBlueprint
    {
        public InjuryRegion Region { get; set; } = InjuryRegion.None;
        public string Name { get; set; }
        public double Severity { get; set; }

        public InjuryBlueprint()
        {
        }

        public InjuryBlueprint(string name, double severity)
        {
            Name = name;
            Severity = severity;
        }

        public InjuryBlueprint(string name, double severity, InjuryRegion region):this(name,severity)
        {
            Region = region;
        }
    }
}
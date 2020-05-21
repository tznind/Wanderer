using Wanderer.Adjectives;
using Wanderer.Systems;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Describes an injury that can be sustained.  This blueprint can be manifested as <see cref="Injured"/> adjective instances
    /// </summary>
    public class InjuryBlueprint
    {
        /// <summary>
        /// The body part that the injury applies to or <see cref="InjuryRegion.None"/> (default)
        /// </summary>
        public InjuryRegion Region { get; set; } = InjuryRegion.None;

        /// <summary>
        /// Player understandable description of the injury e.g. Burnt Arm
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How severe the injury is, relates to <see cref="InjurySystem.FatalThreshold"/> etc
        /// </summary>
        public double Severity { get; set; }

        /// <summary>
        /// Creates a new empty unamed blueprint
        /// </summary>
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
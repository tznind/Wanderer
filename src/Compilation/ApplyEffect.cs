using System;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class ApplyEffect : Effect
    {
        /// <inheritdoc cref="ApplySystemBlueprint.Identifier"/>
        public Guid? Identifier {get;set;}

        /// <inheritdoc cref="ApplySystemBlueprint.Intensity"/>
        public double Intensity { get; set; }

        /// <inheritdoc cref="ApplySystemBlueprint.Name"/>
        public string  Name { get; set; }
        
        /// <inheritdoc cref="ApplySystemBlueprint.All"/>
        public bool All { get; set; }

        /// <summary>
        /// Creates a new effect instance based on the blueprint (<paramref name="apply"/>)
        /// </summary>
        /// <param name="apply">Describes which system to apply, intensity etc</param>
        /// <param name="target">Determines how to pick a target during the apply stage (e.g. always target the Room with the effect)</param>
        public ApplyEffect(ApplySystemBlueprint apply, SystemArgsTarget target) : base(target)
        {
            Identifier = apply.Identifier;
            Intensity = apply.Intensity;
            Name = apply.Name;
            All = apply.All;
        }

        /// <summary>
        /// Looks up the system to apply (by <see cref="Name"/> or <see cref="Identifier"/>) and applies it to the <see cref="Effect.Target"/> in the supplied <paramref name="args"/>
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="GuidNotFoundException"></exception>
        /// <exception cref="NamedObjectNotFoundException"></exception>
        public override void Apply(SystemArgs args)
        {
            ISystem system = Identifier != null ? args.World.GetSystem( Identifier.Value) : args.World.GetSystem(Name);

            var applyTo = args.GetTarget(Target);

            var applyArgs = new SystemArgs(args.World,args.UserInterface,Intensity,args.AggressorIfAny,applyTo,args.Round);
            
            if(All)
                system.ApplyToAll(args.Room.Actors,applyArgs);
            else 
                system.Apply(applyArgs);

        }
    }
}
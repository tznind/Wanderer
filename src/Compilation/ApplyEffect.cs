using System;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class ApplyEffect : Effect
    {
        public Guid? Identifier {get;set;}
        public double Intensity { get; set; }
        public string  Name { get; set; }
        public bool All { get; set; }

        public ApplyEffect(ApplySystemBlueprint apply, SystemArgsTarget target) : base(target)
        {
            Identifier = apply.Identifier;
            Intensity = apply.Intensity;
            Name = apply.Name;
            All = apply.All;
        }

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
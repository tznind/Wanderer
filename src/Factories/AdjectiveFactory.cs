using System;
using System.Linq;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Extensions;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;

namespace Wanderer.Factories
{
    /// <inheritdoc cref="IAdjectiveFactory"/>
    public class AdjectiveFactory : HasStatsFactory<AdjectiveBlueprint,IAdjective>,IAdjectiveFactory
    {
        private readonly TypeCollection _adjectiveTypes;

        /// <summary>
        /// Creates a new instance with no blueprints, note that if you want to create adjectives based on custom Types in your assembly make sure to set <see cref="Compiler.TypeFactory"/> to include your assembly before calling this constructor
        /// </summary>
        public AdjectiveFactory()
        {
            _adjectiveTypes = Compiler.Instance.TypeFactory.Create<IAdjective>();
        }
        
        /// <inheritdoc />
        public IAdjective Create(IWorld world,IHasStats s, AdjectiveBlueprint blueprint)
        {
            HandleInheritance(blueprint);

            var adj = new Adjective(s)
            {
                Name = blueprint.Name
            };
            base.AddBasicProperties(world,adj,blueprint,"inspect");

            if(blueprint.StatsRatio != null)
                adj.StatsRatio = blueprint.StatsRatio;

            if (blueprint.Resist != null)
                adj.Resist = blueprint.Resist;

            adj.IsPrefix = blueprint.IsPrefix;

            s.Adjectives.Add(adj);

            return adj;
        }

        public IAdjective Create(IHasStats s, Type adjectiveType)
        {
            if(!typeof(IAdjective).IsAssignableFrom(adjectiveType))
                throw new ArgumentException($"Expected an IAdjective but was a {adjectiveType}");

            foreach (var constructor in adjectiveType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if(parameters.Length == 1 && parameters[0].ParameterType.IsInstanceOfType(s))
                    {

                        var adj = (IAdjective) constructor.Invoke(new object[]{s});
                        s.Adjectives.Add(adj);
                        return adj;
                    }
            }


            throw new ArgumentException("Could not find a valid constructor for IAdjective Type " + adjectiveType);
        }

        public void AddAdjectives(IWorld world,IHasStats owner, HasStatsBlueprint ownerBlueprint)
        {
            
            //Adjectives the user definitely wants included
            if (ownerBlueprint.MandatoryAdjectives.Any())
                foreach (var a in ownerBlueprint.MandatoryAdjectives)
                    Create(world,owner, a);
            
            //pick 1 random adjective if blueprint lists any to pick from
            if (ownerBlueprint.OptionalAdjectives.Any())
                Create(world,owner, ownerBlueprint.OptionalAdjectives.GetRandom(world.R));
        }

        public IAdjective Create(IWorld world,IHasStats s, Guid guid)
        {
            return Create(world,s, GetBlueprint(guid));
        }

        public IAdjective Create(IWorld world,IHasStats s, string name)
        {
            //when creating by name allow type names too
            var type = _adjectiveTypes.FirstOrDefault(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            return type != null ? Create(s, type) : Create(world,s, GetBlueprint(name));
        }
    }
}
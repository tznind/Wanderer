using System;
using System.Linq;
using Wanderer.Adjectives;
using Wanderer.Compilation;
using Wanderer.Extensions;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;

namespace Wanderer.Factories
{
    public class AdjectiveFactory : HasStatsFactory<AdjectiveBlueprint,IAdjective>,IAdjectiveFactory
    {
        private readonly TypeCollection _adjectiveTypes;

        public AdjectiveFactory()
        {
            _adjectiveTypes = Compiler.Instance.TypeFactory.Create<IAdjective>();
        }
        public IAdjective Create(IHasStats s, AdjectiveBlueprint blueprint)
        {
            var adj = new Adjective(s)
            {
                Name = blueprint.Name
            };
            base.AddBasicProperties(adj,blueprint,"inspect");

            adj.StatsRatio = blueprint.StatsRatio;

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
                    return (IAdjective) constructor.Invoke(new object[]{s});
            }

            throw new ArgumentException("Could not find a valid constructor for IAdjective Type " + adjectiveType);
        }

        public void AddAdjectives(IHasStats owner, HasStatsBlueprint ownerBlueprint, Random r)
        {
            
            //Adjectives the user definitely wants included
            if (ownerBlueprint.MandatoryAdjectives.Any())
                foreach (var a in ownerBlueprint.MandatoryAdjectives)
                    owner.Adjectives.Add(Create(owner, a));
            
            //pick 1 random adjective if blueprint lists any to pick from
            if (ownerBlueprint.OptionalAdjectives.Any())
                owner.Adjectives.Add(
                    Create(owner, ownerBlueprint.OptionalAdjectives.GetRandom(r))
                );
        }

        public IAdjective Create(IHasStats s, Guid guid)
        {
            return Create(s, GetBlueprint(guid));
        }

        public IAdjective Create(IHasStats s, string name)
        {
            //when creating by name allow type names too
            var type = _adjectiveTypes.FirstOrDefault(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            return type != null ? Create(s, type) : Create(s, GetBlueprint(name));
        }
    }
}
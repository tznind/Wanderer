using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Wanderer.Factories;

namespace Tests.Cookbook
{
    class CookBookWorldFactory : WorldFactory
    {
        private readonly List<WorldFactoryResource> _resources;

        public CookBookWorldFactory(List<WorldFactoryResource> resources)
        {
            _resources = resources;
        }

        protected override IEnumerable<WorldFactoryResource> GetResources(string searchPattern)
        {
            return _resources.Where(r => Regex.IsMatch(r.Location,searchPattern.Replace("*",".*")));
        }

        protected override WorldFactoryResource GetResource(string name)
        {
            return 
                _resources.FirstOrDefault(r => r.Location.Equals(name,StringComparison.CurrentCultureIgnoreCase))??
                _resources.FirstOrDefault(r => r.Location.EndsWith(name,StringComparison.InvariantCultureIgnoreCase));
        }
    }
}

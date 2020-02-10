using System.Collections.Generic;
using System.Linq;
using System.Text;
using TB.ComponentModel;
using YamlDotNet.Core;

namespace StarshipWanderer.Compilation
{
    class InstanceBuilder
    {
        public object? Build(ConstructorCollection constructors, params string[] untypedParameters)
        {            
            var useable = constructors.Where(c => c.GetParameters().Length == untypedParameters.Length).ToArray();

            if(useable.Length != 1)
                throw new ParseException($"Found {useable.Length} constructors taking {untypedParameters.Length} parameters for Type '{constructors.Type}'");

            var c = useable.Single();
            var p = c.GetParameters();
            var o = new object[p.Length];

            for (int i = 0; i < p.Length; i++)
            {
                if (string.Equals(untypedParameters[i], "null"))
                    o[i] = null;
                else
                    o[i] = untypedParameters[i].To(p[i].ParameterType);
            }

            return c.Invoke(o);
        }
    }
}

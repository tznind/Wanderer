using System;
using System.Linq;
using StarshipWanderer.Conditions;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Compilation
{
    public class PropertyChain
    {
        public string[] Properties { get; set; }

        public PropertyChain(string chain, out string tail)
        {
            if(!chain.Contains('.'))
                throw new ArgumentException($"PropertyChain must include at least one property (string did not contain any dots - '{chain}')");
            var tokens = chain.Split('.');

            tail = tokens.Last();

            Properties = tokens.Take(tokens.Length - 1).ToArray();
        }

        public IHasStats FollowChain(object o)
        {
            object rootObject = o;
            foreach (string propertyName in Properties)
            {
                var prop = o.GetType().GetProperty(propertyName);

                if(prop == null)
                    throw new ParseException($"Failure following PropertyChain ('{this}') for root object {rootObject}.  Failing link was '{propertyName}'");

                o = prop.GetValue(o);
            }
            
            return o as IHasStats ?? throw new ParseException($"Final leaf of PropertyChain ('{this}') was not an IHasStats (was '{o}')");
        }

        public override string ToString()
        {
            return string.Join("->", Properties);
        }
    }
}
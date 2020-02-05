using System.Linq;
using StarshipWanderer.Conditions;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Compilation
{
    public class PropertyChain
    {
        public string[] Properties { get; set; }

        public PropertyChain(string chain)
        {
            Properties = chain.Split('.').ToArray();
        }

        public IHasStats FollowChain(object o)
        {
            object rootObject = o ?? throw new PropertyChainException("Value passed into FollowChain was null");
            foreach (string propertyName in Properties)
            {
                if(o == null)
                    throw new PropertyChainException($"Encountered null value without exhausting PropertyChain ('{this}')");

                var prop = o.GetType().GetProperty(propertyName);


                if(prop == null)
                    throw new PropertyChainException($"Failure following PropertyChain ('{this}') for root object {rootObject}.  Failing link was '{propertyName}'.  Maybe object Type {o.GetType()} does not have this property?");

                o = prop.GetValue(o);

            }
            
            //if final value is of wrong type
            if(o != null && !(o is IHasStats))
                throw new PropertyChainException($"Final leaf of PropertyChain ('{this}') was not an IHasStats (was '{o}')");

            return (IHasStats) o;
        }

        public override string ToString()
        {
            return string.Join("->", Properties);
        }
    }
}
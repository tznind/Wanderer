using System;
using System.Linq;
using StarshipWanderer.Compilation;

namespace StarshipWanderer.Conditions
{
    /// <summary>
    /// Decorator pattern class which flips the result of the hosted
    /// <see cref="ICondition"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Not<T> : ICondition<T>
    {
        public ICondition<T> Wrapped { get; set; }

        public Not(ICondition<T> wrapped)
        {
            Wrapped = wrapped;
        }

        public bool IsMet(T forTarget)
        {
            return !Wrapped.IsMet(forTarget);
        }

        public string? SerializeAsConstructorCall()
        {
            return "!" + Wrapped.SerializeAsConstructorCall();
        }

        public static object Decorate(ICondition condition)
        {
            
            var iconditionT = condition.GetType()
                .GetInterfaces()
                .Where(i => i.Name == nameof(ICondition) + "`1")
                .ToArray();

            if(iconditionT.Length != 1)
                throw new ArgumentException($"Expected {condition} to implement a single interface ICondition<T> but it implemented {iconditionT.Length}");

            var toBuild = typeof(Not<>).MakeGenericType(iconditionT.Single().GenericTypeArguments.First());

            return toBuild.GetConstructors().Single().Invoke(new object[] {condition});
        }
    }
}
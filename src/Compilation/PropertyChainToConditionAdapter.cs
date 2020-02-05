using System;
using StarshipWanderer.Conditions;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Compilation
{
    /// <summary>
    /// Adapter that turns any ICondition for any Type into an ICondition&lt;IHasStats&gt;
    ///
    /// <para>
    /// This is done by following a specified <see cref="PropertyChain"/> to reach a tail
    /// IHasStats element which is fed into the wrapped underlying ICondition&lt;IHasStats&gt;
    /// </para>
    /// </summary>
    /// <typeparam name="T">The ICondition type you really want e.g. <see cref="SystemArgs"/></typeparam>
    public class PropertyChainToConditionAdapter<T> : ICondition<T>
    {
        private readonly PropertyChain _chain;
        private readonly ICondition<IHasStats> _toWrap;

        public PropertyChainToConditionAdapter(PropertyChain chain, ICondition<IHasStats> toWrap)
        {
            _chain = chain;
            _toWrap = toWrap;
        }

        public bool IsMet(T forTarget)
        {
            return _toWrap.IsMet(_chain.FollowChain(forTarget));
        }

        public string? SerializeAsConstructorCall()
        {
            return string.Join('.',_chain.Properties) + '.' + _toWrap.SerializeAsConstructorCall();
        }
    }
}
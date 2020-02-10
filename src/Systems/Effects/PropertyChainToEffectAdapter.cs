using StarshipWanderer.Compilation;

namespace StarshipWanderer.Effects
{
    /// <summary>
    /// Like <see cref="PropertyChainToConditionAdapter{T}"/> but for <see cref="IEffect{T}"/>
    /// </summary>
    public class PropertyChainToEffectAdapter<T> : IEffect<T>
    {
        private readonly PropertyChain _chain;
        private readonly IEffect<IHasStats> _toWrap;

        public PropertyChainToEffectAdapter(PropertyChain chain, IEffect<IHasStats> toWrap)
        {
            _chain = chain;
            _toWrap = toWrap;
        }

        public void Apply(T forTarget)
        {
            _toWrap.Apply(_chain.FollowChain(forTarget));
        }

        public string? SerializeAsConstructorCall()
        {
            return string.Join('.',_chain.Properties) + '.' + _toWrap.SerializeAsConstructorCall();
        }
    }
}
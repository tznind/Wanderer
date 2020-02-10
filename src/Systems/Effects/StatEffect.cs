using Newtonsoft.Json;
using StarshipWanderer.Conditions;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Effects
{
    class StatEffect<T> : IEffect<IHasStats>
    {
        public Stat Stat { get; }
        public double Bonus { get; }

        [JsonConstructor]
        protected StatEffect()
        {
            
        }

        /// <summary>
        /// Provides a permanent <paramref name="bonus"/> to the given <paramref name="stat"/>
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="bonus"></param>
        public StatEffect(Stat stat, double bonus)
        {
            Stat = stat;
            Bonus = bonus;
        }

        public void Apply(IHasStats forTarget)
        {
            forTarget.BaseStats[Stat] += Bonus;
        }

        public string? SerializeAsConstructorCall()
        {
            return $"StatEffect({Stat},{Bonus})";
        }
    }
}

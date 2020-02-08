using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Stats;

namespace StarshipWanderer
{
    public abstract class HasStats : IHasStats
    {
        private IAdjectiveCollection _adjectives = new AdjectiveCollection();
        private IBehaviourCollection _baseBehaviours = new BehaviourCollection();
        private IActionCollection _baseActions = new ActionCollection();
        public Guid? Identifier { get; set; }
        public bool Unique { get; set; }
        public string Name { get; set; }
        public DialogueInitiation Dialogue { get; set; } = new DialogueInitiation();

        public virtual int Color { get; set; } = DefaultColor;

        /// <summary>
        /// Default Color (white)
        /// </summary>
        public const int DefaultColor = 15;

        public IAdjectiveCollection Adjectives
        {
            get => _adjectives;
            set
            {
                _adjectives = value;
                value?.PruneNulls();

                if(value != null)
                    foreach (var a in value) 
                        a.Owner = this;
            }
        }

        public IActionCollection BaseActions
        {
            get => _baseActions;
            set
            {
                _baseActions = value;
                value?.PruneNulls();
            }
        }

        public StatsCollection BaseStats { get; set; } = new StatsCollection();

        public IBehaviourCollection BaseBehaviours
        {
            get => _baseBehaviours;
            set
            {
                _baseBehaviours = value;
                value?.PruneNulls();

                if(value != null)
                    foreach (var b in value) 
                        b.Owner = this;
            }
        }
        
        public virtual StatsCollection GetFinalStats(IActor forActor)
        {
            var clone = BaseStats.Clone();
            foreach (var adjective in Adjectives) 
                clone.Add(adjective.GetFinalStats(forActor));

            return clone;
        }

        public virtual IActionCollection GetFinalActions(IActor forActor)
        {
            return new ActionCollection(BaseActions.Union(Adjectives.SelectMany(a=>a.GetFinalActions(forActor))));
        }

        public virtual IBehaviourCollection GetFinalBehaviours(IActor forActor)
        {
            return new BehaviourCollection(BaseBehaviours.Union(Adjectives.SelectMany(a=>a.GetFinalBehaviours(forActor))));
        }
        
        public override string ToString()
        {
            return (string.Join(" ", Adjectives.Where(a=>a.IsPrefix)) + " " + (Name ?? "Unnamed Object")).Trim();
        }
        public bool AreIdentical(IHasStats other)
        {
            if (other == null)
                return false;

            //if they are different Types
            if (other.GetType() != GetType())
                return false;
            
            if (!Equals(other.Name,Name))
                return false;

            return other.BaseStats.AreIdentical(BaseStats)
                   && other.BaseActions.AreIdentical(BaseActions)
                   && other.BaseBehaviours.AreIdentical(BaseBehaviours)
                   && other.Adjectives.AreIdentical(Adjectives);
        }

        public virtual IEnumerable<IHasStats> GetAllHaves()
        {
            return Adjectives;
        }
    }
}
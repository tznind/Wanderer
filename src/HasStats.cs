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
        public string Name { get; set; }
        public DialogueInitiation Dialogue { get; set; } = new DialogueInitiation();

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

        public abstract StatsCollection GetFinalStats(IActor forActor);

        public abstract IActionCollection GetFinalActions(IActor forActor);

        public abstract IBehaviourCollection GetFinalBehaviours(IActor forActor);
        
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
    }
}
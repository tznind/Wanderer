using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer
{
    /// <summary>
    /// Abstract base class for all 'things' in the Wanderer world (includes <see cref="Actor"/> and <see cref="Item"/> but also <see cref="Room"/> and even <see cref="Adjective"/> and <see cref="Behaviour"/>
    /// </summary>
    public abstract class HasStats : IHasStats
    {
        /// <inheritdoc />
        public List<IAdjective> Adjectives { get; set; } = new List<IAdjective>();
        /// <inheritdoc />
        public List<IBehaviour> BaseBehaviours { get; set; } = new List<IBehaviour>();
        /// <inheritdoc />
        public List<IAction> BaseActions { get; set; } = new List<IAction>();
        /// <inheritdoc />
        public Guid? Identifier { get; set; }
        /// <inheritdoc />
        public bool Unique { get; set; }
        /// <inheritdoc />
        public string Name { get; set; }
        /// <inheritdoc />
        public IInjurySystem InjurySystem { get; set; }
        /// <inheritdoc />
        public DialogueInitiation Dialogue { get; set; } = new DialogueInitiation();

        /// <inheritdoc />
        public virtual ConsoleColor Color { get; set; } = DefaultColor;

        /// <summary>
        /// Default Color (white)
        /// </summary>
        public const ConsoleColor DefaultColor = ConsoleColor.White;


        public StatsCollection BaseStats { get; set; } = new StatsCollection();
        
        /// <summary>
        /// Custom variables for user scripts and tracking counts
        /// </summary>
        public DynamicDictionary V {get;set;} = new DynamicDictionary();
        
        public virtual StatsCollection GetFinalStats(IActor forActor)
        {
            var clone = BaseStats.Clone();
            foreach (var adjective in Adjectives) 
                clone.Increase(adjective.GetFinalStats(forActor));
            
            foreach(var adjective in Adjectives)
                clone = adjective.Modify(clone);

            return clone;
        }

        public StatsCollection GetFinalStats(Frame frame)
        {
            return GetFinalStats(frame.PerformedBy)
                .Increase(frame.Action.BaseStats);
        }

        public virtual List<IAction> GetFinalActions(IActor forActor)
        {
            return new List<IAction>(BaseActions.Union(Adjectives.SelectMany(a=>a.GetFinalActions(forActor))));
        }

        public virtual List<IBehaviour> GetFinalBehaviours(IActor forActor)
        {
            return new List<IBehaviour>(BaseBehaviours.Union(Adjectives.SelectMany(a=>a.GetFinalBehaviours(forActor))));
        }
        
        public override string ToString()
        {
            return (string.Join(" ", Adjectives.Where(a=>a.IsPrefix)) + " " + (Name ?? "Unnamed Object")).Trim();
        }
        public virtual bool AreIdentical(object other)
        {
            if (other == null)
                return false;

            //if they are different Types
            if (other.GetType() != GetType())
                return false;

            if (other is IHasStats hasStats)
            {
                if (!Equals(hasStats.Name,Name))
                    return false;

                return hasStats.BaseStats.AreIdentical(BaseStats)
                       && hasStats.BaseActions.AreIdentical(BaseActions)
                       && hasStats.BaseBehaviours.AreIdentical(BaseBehaviours)
                       && hasStats.Adjectives.AreIdentical(Adjectives);
            }
            
            return false;
        }

        public virtual IEnumerable<IHasStats> GetAllHaves()
        {
            return Adjectives.Cast<IHasStats>().Union(BaseActions);
        }

        public List<IHasStats> Get(Guid? guid)
        {
            return GetAllHaves().Where(h => h.Is(guid)).ToList();
        }
        
        public List<IHasStats> Get(string name)
        {
            return GetAllHaves().Where(h => h.Is(name)).ToList();
        }

        public bool Has(Guid? g)
        {
            return g.HasValue && (Is(g) || GetAllHaves().Any(h=>h.Is(g)));
        }

        public bool Has(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (Guid.TryParse(name, out Guid g))
                return Has(g);

            return Is(name) || GetAllHaves().Any(h=>h.Is(name));
        }

        public bool Is(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (Guid.TryParse(name, out Guid g))
                return Is(g);

            return
            string.Equals(GetType().Name,name,StringComparison.CurrentCultureIgnoreCase)
            ||
             string.Equals(Name, name, StringComparison.CurrentCultureIgnoreCase);
        }

        public bool Is(Guid? g)
        {
            return g.HasValue && Equals(Identifier, g);
        }
    }
}
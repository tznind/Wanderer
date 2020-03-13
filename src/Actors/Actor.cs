using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Wanderer.Actions;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Relationships;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Actors
{
    /// <inheritdoc cref="IActor"/>
    public abstract class Actor : HasStats,IActor
    {
        public bool Dead { get; set; }

        /// <inheritdoc/>
        public IRoom CurrentLocation { get; set; }
        
        public List<IItem> Items { get; set; } = new List<IItem>();
        public HashSet<IFaction> FactionMembership { get; set; } = new HashSet<IFaction>();

        public SlotCollection AvailableSlots { get; set; } = new SlotCollection();
        
        private ConsoleColor _explicitColor = DefaultColor;

        public override ConsoleColor Color
        {
            //use the faction color unless we have an explicit room color set
            get =>
                _explicitColor != DefaultColor ? _explicitColor : FactionMembership.FirstOrDefault()?.Color ?? _explicitColor;
            set => _explicitColor = value;
        } 

        /// <summary>
        /// Do not use, internal constructor for JSON serialization
        /// </summary>
        [JsonConstructor]
        protected Actor()
        {

        }

        /// <summary>
        /// Creates a new actor with the given <paramref name="name"/> and adds him to the <paramref name="currentLocation"/> (and world population)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="currentLocation"></param>
        public Actor(string name,IRoom currentLocation)
        {
            Name = name;
            CurrentLocation = currentLocation;
            CurrentLocation.World.Population.Add(this);
            
            //basic actions everyone can do (by default)
            BaseActions.Add(new LeaveAction());

            //if there are ways to be injured then there are ways to fight
            if(currentLocation.World.InjurySystems.Any())
                BaseActions.Add(new FightAction());

            BaseActions.Add(new PickUpAction());
            BaseActions.Add(new DropAction());
            BaseActions.Add(new GiveAction());
            BaseActions.Add(new DialogueAction());
            BaseActions.Add(new EquipmentAction());

            BaseBehaviours.Add(new MergeStacksBehaviour(this));

            //TODO: this is a reference to the hunger system which means this behaviour should go into yaml too
            var hungerSystem = CurrentLocation.World.InjurySystems.FirstOrDefault(i=>i.Identifier == new Guid("89c18233-5250-4445-8799-faa9a888fb7f"));

            if(hungerSystem != null)
                BaseBehaviours.Add(new GetsHungryBehaviour(this,hungerSystem));
        }

        /// <summary>
        /// Returns all <see cref="IAction"/> which the <see cref="Actor"/> can undertake in it's <see cref="CurrentLocation"/> (this includes 
        /// <see cref="HasStats.BaseActions"/> but also any location or item specific actions.
        /// </summary>
        /// <param name="forActor"></param>
        /// <returns></returns>
        public override IActionCollection GetFinalActions(IActor forActor)
        {
            return new ActionCollection(BaseActions
                .Union(Adjectives.SelectMany(a => a.GetFinalActions(forActor)))
                .Union(CurrentLocation.GetFinalActions(forActor))
                .Union(FactionMembership.SelectMany(f=>f.GetFinalActions(forActor)))
                .Union(Items.SelectMany(i => i.GetFinalActions(forActor))));
        }

        public abstract bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options,
            double attitude);

        public virtual void Move(IRoom newLocation)
        {
            CurrentLocation = newLocation;
        }

        public abstract void Kill(IUserinterface ui, Guid round, string reason);

        /// <inheritdoc/>
        public IActor[] GetCurrentLocationSiblings(bool includeDead)
        {
            return CurrentLocation.World.Population.Where(o => o.CurrentLocation == CurrentLocation && o != this && (!o.Dead || includeDead)).ToArray();
        }

        public bool Has<T>(bool includeItems) where T : IAdjective
        {
            return Adjectives.Union(FactionMembership.SelectMany(f=>f.Adjectives)).Any(a => a is T)
                || includeItems && Items.Any(i=> i.Has<T>(this));
        }

        public bool Has<T>(bool includeItems, Func<T, bool> condition) where T : IAdjective
        {
            return Adjectives.Any(a => a is T t  && condition(t))
                || includeItems && Items.Any(i => i.Has<T>(this,condition));
        }

        public override IBehaviourCollection GetFinalBehaviours(IActor forActor)
        {
            //the dead have no behaviours
            if(Dead)
                return new BehaviourCollection();

            return new BehaviourCollection(BaseBehaviours
                .Union(Adjectives.SelectMany(a=>a.GetFinalBehaviours(forActor)))
                .Union(CurrentLocation.GetFinalBehaviours(forActor))
                .Union(FactionMembership.SelectMany(f=>f.GetFinalBehaviours(forActor)))
                .Union(Items.SelectMany(i=>i.GetFinalBehaviours(forActor))));
        }

        public override StatsCollection GetFinalStats(IActor forActor)
        {
            var clone = BaseStats.Clone();

            foreach (var adjective in Adjectives) 
                clone.Add(adjective.GetFinalStats(forActor));

            clone.Add(CurrentLocation.GetFinalStats(forActor));

            foreach (var faction in FactionMembership) 
                clone.Add(faction.GetFinalStats(forActor));

            foreach (var item in Items) 
                clone.Add(item.GetFinalStats(forActor));

            return clone;
        }
        
        public StatsCollection GetFinalStats()
        {
            return GetFinalStats(this);
        }

        
        public IEnumerable<IAction> GetFinalActions()
        {
            return GetFinalActions(this);
        }

        
        public IEnumerable<IBehaviour> GetFinalBehaviours()
        {
            return GetFinalBehaviours(this);
        }

        public bool IsAwareOf(IActor other)
        {
            return CurrentLocation.GetPoint() == other.CurrentLocation.GetPoint();
        }

        public virtual bool CanEquip(IItem item,out string reason)
        {
            //already equipped, dead etc
            if (item.IsEquipped)
            {
                reason = "Already equipped";
                return false;
            }

            if (item.Slot == null)
            {
                reason = "Item cannot be equipped";
                return false;
            }

            if (!AvailableSlots.ContainsKey(item.Slot.Name))
            {
                reason = $"You do not have a {item.Slot.Name} slot";
                return false;
            }

            var alreadyWearing = Items.Where(i => i.IsEquipped && i.Slot != null && i.Slot.Name == item.Slot.Name);
            var alreadyWearingCount = alreadyWearing.Sum(i => i.Slot.NumberRequired);

            if (AvailableSlots[item.Slot.Name] < item.Slot.NumberRequired + alreadyWearingCount)
            {
                reason = $"You do not have enough free {item.Slot.Name} slots";
                return false;
            }

            if (!item.CanUse(this,out reason))
                return false;

            reason = null;
            return true;
        }

        public double AttitudeTo(IActor other)
        {
            return this.CurrentLocation.World.Relationships.SumBetween(this,other);
        }

        public override string ToString()
        {
            return (Dead ? "Dead ":"") + Name;
        }

        public override IEnumerable<IHasStats> GetAllHaves()
        {
            if(Dead)
                return new IHasStats[0];

            return 
                base.GetAllHaves()
                    .Union(Items)
                    .Union(Items.SelectMany(i=>i.GetAllHaves()));
        }

        public double DistanceTo(IActor actor)
        {
            var world = CurrentLocation.World;

            return world.Map.GetPoint(CurrentLocation).Distance(world.Map.GetPoint(actor.CurrentLocation));
        }

        public IActor BestFriend(bool inSameLocation, double threshold)
        {
            var world = CurrentLocation.World;
            var relationships = world.Relationships;
            var consider = inSameLocation ? 
                    GetCurrentLocationSiblings(false)
                    : world.Population.ToArray();

            return consider.OrderByDescending(a=>relationships.SumBetween(this,a))
                            .Where(a=>relationships.SumBetween(this,a) > threshold)
                            .FirstOrDefault();
        }

        public IActor WorstEnemy(bool inSameLocation, double threshold)
        {
            var world = CurrentLocation.World;
            var relationships = world.Relationships;
            var consider = inSameLocation ? 
                    GetCurrentLocationSiblings(false)
                    : world.Population.ToArray();

            return consider.OrderBy(a=>relationships.SumBetween(this,a))
                            .Where(a=>relationships.SumBetween(this,a) < threshold)
                            .FirstOrDefault();
        }

        public IItem SpawnItem(ItemBlueprint blue)
        {
            return SpawnItem(CurrentLocation.World.ItemFactory.Create(CurrentLocation.World,blue));
        }

        public IItem SpawnItem(Guid g)
        {
            return SpawnItem(CurrentLocation.World.ItemFactory.Create(CurrentLocation.World,g));
        }
        public IItem SpawnItem(string name)
        {
            return SpawnItem(CurrentLocation.World.ItemFactory.Create(CurrentLocation.World,name));
        }
        
        protected virtual IItem SpawnItem(IItem item)
        {
            Items.Add(item);
            return item;
        }
        
        public void Equip(IItem item)
        {
            if (CanEquip(item, out _))
                item.IsEquipped = true;
        }
    }
}
